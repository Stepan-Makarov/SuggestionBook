using SuggestionBookLibrary.Models.SubModels;

namespace SuggestionBookLibrary.Models;
public class SuggestionModel
{
  [BsonId]
  [BsonRepresentation(BsonType.ObjectId)]
  public string Id { get; set; }
  public string Title { get; set; }
  public string Description { get; set; }
  public DateTime DateCreated { get; set; } = DateTime.UtcNow;
  public CategoryModel Category { get; set; }
  public UserSubModel Author { get; set; }
  public HashSet<string> UserVotes { get; set; } = new();
  public StatusModel Status { get; set; }
  public string OwnerNotes { get; set; }
  public bool ApprovedForRelease { get; set; } = false;
  public bool Rejected { get; set; } = false;
  public bool Archived { get; set; } = false;
}
