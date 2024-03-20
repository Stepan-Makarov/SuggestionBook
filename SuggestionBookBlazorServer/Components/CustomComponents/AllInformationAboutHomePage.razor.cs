using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace SuggestionBookBlazorServer.Components.CustomComponents;
public partial class AllInformationAboutHomePage
{
  private List<SuggestionModel>? suggestions;
  private List<SuggestionModel>? archivedSuggestions;
  [Parameter]
  public List<CategoryModel>? Categories { get; set; }
  [Parameter]
  public List<StatusModel>? Statuses { get; set; }
  [Parameter]
  public string? LoggedInUserId { get; set; }
  [CascadingParameter]
  private Task<AuthenticationState>? authenticationState { get; set; }

  private SuggestionModel? archivingSuggestion;
  private string selectedCategory = "All";
  private string selectedStatus = "All";
  private string searchText = "";
  private bool isSortedByNew = false;
  private bool showCategories = false;
  private bool showStatuses = false;

  protected async override Task OnAfterRenderAsync(bool firstRender)
  {
    if (firstRender)
    {
      await LoadFilterState();
      await FilterSuggestions();
      StateHasChanged();
    }
  }

  private async Task LoadFilterState()
  {
    var stringResult = await SessionStorage.GetAsync<string>(nameof(selectedCategory));
    selectedCategory = stringResult.Success ? stringResult.Value! : "All";

    stringResult = await SessionStorage.GetAsync<string>(nameof(selectedStatus));
    selectedStatus = stringResult.Success ? stringResult.Value! : "All";

    stringResult = await SessionStorage.GetAsync<string>(nameof(searchText));
    searchText = stringResult.Success ? stringResult.Value! : "";

    var boolResult = await SessionStorage.GetAsync<bool>(nameof(isSortedByNew));
    isSortedByNew = boolResult.Success ? boolResult.Value : false;
  }

  private async Task<bool> IsUserAdmin()
  {
    if (authenticationState is not null)
    {
      var authState = await authenticationState;
      var user = authState?.User;

      if (user is not null)
      {
        if (user.IsInRole("Admin"))
        {
          return true;
        }
      }
    }
    return false;
  }

  private async Task FilterSuggestions()
  {
    var output = await SuggestionData.GetAllApprovedSuggestions();

    if (await IsUserAdmin() == true)
    {
      archivedSuggestions = await SuggestionData.GetAllArchivedSuggestions();
      foreach (var archivedSuggestion in archivedSuggestions)
      {
        output.Add(archivedSuggestion);
      }
    }

    if (selectedCategory != "All")
    {
      output = output.Where(s => s.Category?.CategoryName == selectedCategory).ToList();
    }

    if (selectedStatus != "All")
    {
      output = output.Where(s => s.Status?.StatusName == selectedStatus).ToList();
    }

    if (string.IsNullOrWhiteSpace(searchText) == false)
    {
      output = output.Where(
          s => (s.Title.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)) ||
               (s.Description.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))).ToList();
    }

    if (isSortedByNew)
    {
      output = output.OrderByDescending(s => s.DateCreated).ToList();
    }

    else
    {
      output = output.OrderByDescending(s => s.UserVotes?.Count)
                     .ThenByDescending(s => s.DateCreated).ToList();
    }

    suggestions = output;

    await SaveFilterState();
  }

  private async Task SaveFilterState()
  {
    await SessionStorage.SetAsync(nameof(selectedCategory), selectedCategory);
    await SessionStorage.SetAsync(nameof(selectedStatus), selectedStatus);
    await SessionStorage.SetAsync(nameof(searchText), searchText);
    await SessionStorage.SetAsync(nameof(isSortedByNew), isSortedByNew);
  }

  private async Task OrderByNew(bool isNew)
  {
    isSortedByNew = isNew;
    await FilterSuggestions();
  }

  private async Task OnCategoryClick(string category = "All")
  {
    selectedCategory = category;
    showCategories = false;
    await FilterSuggestions();
  }

  private async Task OnStatusClick(string status = "All")
  {
    selectedStatus = status;
    showStatuses = false;
    await FilterSuggestions();
  }

  private async Task OnSearchInput(string searchInput)
  {
    searchText = searchInput;
    await FilterSuggestions();
  }

  private void OpenDetails(SuggestionModel suggestion)
  {
    NavMagager.NavigateTo($"Details/{suggestion.Id}");
  }

  private void GoToCreateSuggestion()
  {
    NavMagager.NavigateTo("Create");
  }

  private string SortedByNewClass(bool isNew)
  {
    if (isNew == isSortedByNew)
    {
      return "sort-selected";
    }
    else
    {
      return "";
    }
  }

  private string SuggestionStatusClass(SuggestionModel suggestion)
  {
    if (suggestion.Status == null || suggestion is null)
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

  private string GetSelectedCategory(string categoryName = "All")
  {
    if (categoryName == selectedCategory)
    {
      return "selected-category";
    }

    else
    {
      return "";
    }
  }

  private string GetSelectedStatus(string statusName = "All")
  {
    if (statusName == selectedStatus)
    {
      return "selected-status";
    }

    else
    {
      return "";
    }
  }

  private string GetArchivedButtonText(SuggestionModel suggestion)
  {
    return suggestion.Archived ? "Из архива" : "В архив";
  }

  private async Task ArchiveSuggestion()
  {
    if (archivingSuggestion!.Archived == false)
    {
      archivingSuggestion.Archived = true;
      await SuggestionData.UpdateSuggestion(archivingSuggestion);
      // suggestions?.Remove(archivingSuggestion);
      // archivedSuggestions?.Add(archivingSuggestion);
      archivingSuggestion = null;
    }
    else
    {
      archivingSuggestion.Archived = false;
      await SuggestionData.UpdateSuggestion(archivingSuggestion);
      // suggestions?.Add(archivingSuggestion);
      // archivedSuggestions?.Remove(archivingSuggestion);
      archivingSuggestion = null;
    }
  }
}