namespace SuggestionBookLibrary.DataAccess.IDataAccess;

public interface IUserData
{
  Task CreateUser(UserModel user);
  Task<List<UserModel>> GetAllUsers();
  Task<UserModel> GetUserByAuthId(string objectId);
  Task<UserModel> GetUserById(string id);
  Task UpdateUser(UserModel user);
}