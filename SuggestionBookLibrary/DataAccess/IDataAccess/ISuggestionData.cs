namespace SuggestionBookLibrary.DataAccess.IDataAccess;

public interface ISuggestionData
{
  Task CreateSuggestion(SuggestionModel suggestion);
  Task<List<SuggestionModel>> GetAllApprovedSuggestions();
  Task<List<SuggestionModel>> GetAllArchivedSuggestions();
  Task<List<SuggestionModel>> GetAllSuggestions();
  Task<List<SuggestionModel>> GetAllUsersSuggestions(string userId);
  Task<List<SuggestionModel>> GetAllWaitingForApprovalSuggestions();
  Task<SuggestionModel> GetSuggestion(string id);
  Task UpdateSuggestion(SuggestionModel suggestion);
  Task UpvoteSuggestion(string suggestionId, string userId);
}