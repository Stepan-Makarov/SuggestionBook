using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace SuggestionBookBlazorServer.Components.Pages;
public partial class SuggestionDetails
{
  [Parameter]
  public string? Id { get; set; }

  [CascadingParameter]
  private Task<AuthenticationState>? authenticationState { get; set; }

  private UserModel? loggedInUser;
  private SuggestionModel? suggestion;
  private List<StatusModel>? statuses;
  private List<UserModel>? users;

  protected async override Task OnInitializedAsync()
  {
    suggestion = await SuggestionData.GetSuggestion(Id);
    users = await UserData.GetAllUsers();
    {
      await GetAndVerifyUser();
    }

    statuses = await StatusData.GetAllStatuses();
  }

  protected async Task HandleStatusChanged()
  {
    await InvokeAsync(StateHasChanged);
  }

  private async Task<UserModel> GetAndVerifyUser()
  {
    if (authenticationState is not null)
    {
      var authState = await authenticationState;
      var user = authState?.User;

      if (user?.Identity is not null && user.Identity.IsAuthenticated)
      {
        loggedInUser = users?.Where(u => u.EmailAddress == user.Identity.Name).FirstOrDefault();
        // var usr = users?.Where(u => u.EmailAddress == user.Identity.Name).FirstOrDefault();
        // loggedInUser = await UserData.GetUserByAuthId(usr!.ObjectIdentifier);
        return loggedInUser!;
      }
    }
    return loggedInUser = null!;
  }

  private string SuggestionStatusClass()
  {
    if (suggestion is null || suggestion.Status == null)
    {
      return "suggestion-entry-status-none";
    }

    string output = suggestion.Status.StatusName switch
    {
      "Завершено" => "suggestion-entry-status-completed",
      "На рассмотрении" => "suggestion-entry-status-watching",
      "В производстве" => "suggestion-entry-status-upcoming",
      "Отклонено" => "suggestion-entry-status-dismissed",
      _ => "suggestion-entry-status-none"
    };

    return output;
  }
}