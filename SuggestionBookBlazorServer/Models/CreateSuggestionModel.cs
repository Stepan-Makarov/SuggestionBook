using System.ComponentModel.DataAnnotations;

namespace SuggestionBookBlazorServer.Models;

public class CreateSuggestionModel
{
  [Required]
  [MaxLength(100)]
  public string Title { get; set; }

  [Required]
  [MinLength(1)]
  [Display(Name = "Category")]
  public string CategoryId { get; set; }

  [MaxLength(500)]
  public string Description { get; set; }
}
