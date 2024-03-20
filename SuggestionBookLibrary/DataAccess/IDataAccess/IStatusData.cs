namespace SuggestionBookLibrary.DataAccess.IDataAccess;

public interface IStatusData
{
  Task CreateStatus(StatusModel status);
  Task DeleteStatus(string statusId);
  Task<List<StatusModel>> GetAllStatuses();
  Task UpdateStatus(StatusModel status);
}