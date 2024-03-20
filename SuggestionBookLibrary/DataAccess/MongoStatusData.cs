using Microsoft.Extensions.Caching.Memory;

namespace SuggestionBookLibrary.DataAccess;
public class MongoStatusData : IStatusData
{
  private readonly IMongoCollection<StatusModel> _statuses;
  private readonly IMemoryCache _cache;
  private const string CacheName = "StatusData";

  public MongoStatusData(INoSqlDbConnection db, IMemoryCache cache)
  {
    _cache = cache;
    _statuses = db.StatusCollection;
  }

  public async Task<List<StatusModel>> GetAllStatuses()
  {
    var output = _cache.Get<List<StatusModel>>(CacheName);

    if (output == null)
    {
      var results = await _statuses.FindAsync(_ => true);
      output = results.ToList();

      _cache.Set(CacheName, output, TimeSpan.FromDays(1));
    }
    return output;
  }

  public async Task CreateStatus(StatusModel status)
  {
    await _statuses.InsertOneAsync(status);
    _cache.Remove(CacheName);
  }

  public async Task UpdateStatus(StatusModel status)
  {
    var filter = Builders<StatusModel>.Filter.Eq("Id", status.Id);
    await _statuses.ReplaceOneAsync(filter, status);
    _cache.Remove(CacheName);
  }

  public async Task DeleteStatus(string statusId)
  {
    await _statuses.DeleteOneAsync(u => u.Id == statusId);
    _cache.Remove(CacheName);
  }
}
