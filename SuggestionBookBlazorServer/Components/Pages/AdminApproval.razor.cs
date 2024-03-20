namespace SuggestionBookBlazorServer.Components.Pages;
public partial class AdminApproval
{
  private List<StatusModel>? statuses;
  private List<SuggestionModel>? submissions;
  private string currentEditingTitle = "";
  private string editedTitle = "";
  private string currentEditingDescription = "";
  private string editedDescription = "";

  protected async override Task OnInitializedAsync()
  {
    submissions = await suggestionData.GetAllWaitingForApprovalSuggestions();
  }

  private async Task ApproveSuggestion(SuggestionModel submission)
  {
    submission.ApprovedForRelease = true;
    submissions?.Remove(submission);
    await suggestionData.UpdateSuggestion(submission);
  }

  private async Task RejectSuggestion(SuggestionModel submission)
  {
    statuses = await statusData.GetAllStatuses();
    submission.Rejected = true;
    if (statuses?.Count > 0)
    {
      submission.Status = statuses.Where(s => s.StatusName == "Отклонено").FirstOrDefault();
    }
    submissions?.Remove(submission);
    await suggestionData.UpdateSuggestion(submission);
  }

  private void EditTitle(SuggestionModel model)
  {
    editedTitle = model.Title;
    currentEditingTitle = model.Id;
    currentEditingDescription = "";
  }

  private async Task SaveTitle(SuggestionModel model)
  {
    currentEditingTitle = "";
    model.Title = editedTitle;
    await suggestionData.UpdateSuggestion(model);
  }

  private void EditDescription(SuggestionModel model)
  {
    if (model.Description is not null)
    {
      editedDescription = model.Description;
    }
    else
    {
      editedDescription = "";
    }
    currentEditingDescription = model.Id;
    currentEditingTitle = "";
  }

  private async Task SaveDescription(SuggestionModel model)
  {
    currentEditingDescription = "";
    model.Description = editedDescription;
    await suggestionData.UpdateSuggestion(model);
  }
}