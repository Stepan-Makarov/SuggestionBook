using Microsoft.Extensions.Caching.Memory;

namespace SuggestionBookLibrary.DataAccess;
public class MongoSuggestionData : ISuggestionData
{
  private readonly INoSqlDbConnection _db;
  private readonly IUserData _userData;
  private readonly IMemoryCache _cache;
  private readonly IMongoCollection<SuggestionModel> _suggestions;
  private const string CacheName = "SuggestionData";

  public MongoSuggestionData(INoSqlDbConnection db, IUserData userData, IMemoryCache cache)
  {
    _db = db;
    _userData = userData;
    _cache = cache;
    _suggestions = db.SuggestionCollection;
  }

  public async Task<List<SuggestionModel>> GetAllSuggestions()
  {
    var output = _cache.Get<List<SuggestionModel>>(CacheName);

    if (output == null)
    {
      var results = await _suggestions.FindAsync(s => s.Archived == false);
      output = results.ToList();

      _cache.Set(CacheName, output, TimeSpan.FromMinutes(1));
    }
    return output;
  }

  public async Task<List<SuggestionModel>> GetAllArchivedSuggestions()
  {
    var results = await _suggestions.FindAsync(s => s.Archived == true);
    var output = results.ToList();
    return output;
  }

  public async Task<List<SuggestionModel>> GetAllUsersSuggestions(string userId)
  {
    var output = _cache.Get<List<SuggestionModel>>(userId);
    if (output == null)
    {
      var results = await _suggestions.FindAsync(s=> s.Author.Id == userId);
      output = results.ToList();

      _cache.Set(userId, output, TimeSpan.FromMinutes(1));
    }
    return output;
  }

  public async Task<List<SuggestionModel>> GetAllApprovedSuggestions()
  {
    var output = await GetAllSuggestions();
    return output.Where(s => s.ApprovedForRelease == true).ToList();
  }

  public async Task<List<SuggestionModel>> GetAllWaitingForApprovalSuggestions()
  {
    var output = await GetAllSuggestions();
    var results = output.Where(s => s.ApprovedForRelease == false && s.Rejected == false).ToList();
    return results;
  }

  public async Task<SuggestionModel> GetSuggestion(string id)
  {
    var results = await _suggestions.FindAsync(s => s.Id == id);
    var output = results.FirstOrDefault();
    return output;
  }

  public async Task UpdateSuggestion(SuggestionModel suggestion)
  {
    await _suggestions.ReplaceOneAsync(s => s.Id == suggestion.Id, suggestion);
    _cache.Remove(CacheName);
  }

  public async Task UpvoteSuggestion(string suggestionId, string userId)
  {
    var client = _db.Client;

    using var session = await client.StartSessionAsync();

    session.StartTransaction();

    try
    {
      var db = client.GetDatabase(_db.DbName);
      var suggestionsInTransaction = db.GetCollection<SuggestionModel>(_db.SuggestionCollectionName);
      var suggestion = (await suggestionsInTransaction.FindAsync(s => s.Id == suggestionId)).First();
      bool isUpvoted = suggestion.UserVotes.Add(userId);

      if (isUpvoted == false)
      {
        suggestion.UserVotes.Remove(userId);
      }

      await suggestionsInTransaction.ReplaceOneAsync(session, s => s.Id == suggestionId, suggestion);

      var usersInTransaction = db.GetCollection<UserModel>(_db.UserCollectionName);
      var user = await _userData.GetUserById(userId);

      if (isUpvoted)
      {
        user.VotedSuggestions.Add(new SuggestionSubModel(suggestion));
      }
      else
      {
        var suggestionToRemove = user.VotedSuggestions.Where(s => s.Id == suggestionId).First();
        user.VotedSuggestions.Remove(suggestionToRemove);
      }

      await usersInTransaction.ReplaceOneAsync(session, u => u.Id == userId, user);

      await session.CommitTransactionAsync();

      _cache.Remove(CacheName);
    }
    catch (Exception)
    {
      await session.AbortTransactionAsync();
      throw;
    }
  }

  public async Task CreateSuggestion(SuggestionModel suggestion)
  {
    var client = _db.Client;

    using var session = await client.StartSessionAsync();

    session.StartTransaction();

    try
    {
      var db = client.GetDatabase(_db.DbName);
      var suggestionsInTransaction = db.GetCollection<SuggestionModel>(_db.SuggestionCollectionName);
      await suggestionsInTransaction.InsertOneAsync(session, suggestion);

      var usersInTransaction = db.GetCollection<UserModel>(_db.UserCollectionName);
      var user = await _userData.GetUserById(suggestion.Author.Id);
      user.AuthoredSuggestions.Add(new SuggestionSubModel(suggestion));
      await usersInTransaction.ReplaceOneAsync(session, u => u.Id == user.Id, user);

      await session.CommitTransactionAsync();
    }
    catch (Exception)
    {
      await session.AbortTransactionAsync();
      throw;
    }
  }
}
