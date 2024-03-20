using Microsoft.AspNetCore.Components;

namespace SuggestionBookBlazorServer.Components.Shared;
public partial class ApvoteComponent
{
  [Parameter]
  public string? LoggedInUserId { get; set; }
  [Parameter]
  public SuggestionModel? Suggestion { get; set; }

  private bool loading;
  private Task showTask = Task.CompletedTask;

  private string GetUpvoteTopText(SuggestionModel suggestion)
  {
    if (suggestion.UserVotes?.Count > 0)
    {
      return suggestion.UserVotes.Count.ToString("00");
    }

    else
    {
      if (LoggedInUserId == suggestion.Author.Id)
      {
        return "Невозможно";
      }
      return "Нажмите чтобы";
    }
  }

  private string GetUpvoteBottomText(SuggestionModel suggestion)
  {
    var votes = suggestion?.UserVotes.Count;
    //разбиваем число голосов на цифры
    var digits = votes?.ToString().Select(c => c - '0');

    if (digits != null)
    {
      int lastDigit = digits.Last();
      int lastTwoDigit = int.Parse(string.Concat(digits.TakeLast(2)));

      if ((votes > 0) && (lastTwoDigit >= 11 && lastTwoDigit <= 14))
      {
        return "голосов";
      }

      if ((votes > 0) && (lastDigit == 1))
      {
        return "голос";
      }

      if ((votes > 0) && (lastDigit >= 2 && lastDigit <= 4))
      {
        return "голоса";
      }

      if ((votes > 0) && ((lastDigit >= 5 && lastDigit <= 9) || (lastDigit == 0)))
      {
        return "голосов";
      }

      else
      {
        return "проголосовать";
      }
    }
    else
    {
      return "проголосовать";
    }
  }

  private async Task UpvoteSuggestion(SuggestionModel suggestion)
  {
    if (LoggedInUserId is not null)
    {
      if (loading == false)
      {
        loading = true;
        if (LoggedInUserId == suggestion.Author.Id)
        {
          return;
        }
        if (suggestion.UserVotes.Add(LoggedInUserId) == false)
        {
          suggestion.UserVotes.Remove(LoggedInUserId);
        }

        await SuggestionData.UpvoteSuggestion(suggestion.Id, LoggedInUserId);

        await Task.Delay(500);
        loading = false;
      }
    }
    else
    {
      NavMagager.NavigateTo("Account/Login");
    }

  }

  private string GetVoteClass(SuggestionModel suggestion)
  {
    if (suggestion.UserVotes is null || suggestion.UserVotes.Count == 0)
    {
      return "suggestion-no-votes";
    }

    else if (suggestion.UserVotes.Contains(LoggedInUserId))
    {
      return "suggestion-voted";
    }

    else
    {
      return "suggestion-not-voted";
    }
  }
}