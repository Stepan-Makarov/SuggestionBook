using Microsoft.AspNetCore.Components;

namespace SuggestionBookBlazorServer.Components.CustomComponents;
public partial class SetStatusComponent
{
  private string? settingStatus;
  private string? settingAdminNote;

  [Parameter]
  public SuggestionModel? Suggestion { get; set; }
  [Parameter]
  public List<StatusModel>? Statuses { get; set; }
  [Parameter]
  public EventCallback OnStatusChanged { get; set; }

  private async Task CompleteSetStatus()
  {
    Suggestion!.Status = Statuses?.Where(s => s.StatusName.ToLower() == settingStatus?.ToLower()).First();
    Suggestion!.OwnerNotes = settingAdminNote;

    settingStatus = null;
    settingAdminNote = null;

    await SuggestionData.UpdateSuggestion(Suggestion);
    await OnStatusChanged.InvokeAsync();
  }

  private void CancelSettingStatus()
  {
    settingStatus = null;
    settingAdminNote = null;
  }

  private string SuggestionStatusClass()
  {
    if (settingStatus is null)
    {
      return "";
    }

    string output = settingStatus switch
    {
      "���������" => "btn-status-completed",
      "�� ������������" => "btn-status-watching",
      "� ������������" => "btn-status-upcoming",
      "���������" => "btn-status-dismissed",
      _ => ""
    };

    return output;
  }
}