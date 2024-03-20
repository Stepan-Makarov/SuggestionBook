namespace SuggestionBookLibrary.DataAccess;
public class MongoUserData : IUserData
{
  private readonly IMongoCollection<UserModel> _users;

  public MongoUserData(INoSqlDbConnection db)
  {
    _users = db.UserCollection;
  }

  public async Task<List<UserModel>> GetAllUsers()
  {
    var results = await _users.FindAsync(_ => true);
    return results.ToList();
  }

  public async Task<UserModel> GetUserById(string id)
  {
    var results = await _users.FindAsync(u => u.Id == id);
    return results.FirstOrDefault();
  }

  public async Task<UserModel> GetUserByAuthId(string objectId)
  {
    var results = await _users.FindAsync(u => u.ObjectIdentifier == objectId);
    return results.FirstOrDefault();
  }

  public Task CreateUser(UserModel user)
  {
    var result = _users.InsertOneAsync(user);
    return result;
  }

  public Task UpdateUser(UserModel user)
  {
    var filter = Builders<UserModel>.Filter.Eq("Id", user.Id);
    var result = _users.ReplaceOneAsync(filter, user, new ReplaceOptions { IsUpsert = true });
    return result;
  }
}
