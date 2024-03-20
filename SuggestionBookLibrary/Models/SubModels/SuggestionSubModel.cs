namespace SuggestionBookLibrary.Models.SubModels;
public class SuggestionSubModel
{
  [BsonRepresentation(BsonType.ObjectId)]
  public string Id { get; set; }
  public string Title { get; set; }

  public SuggestionSubModel()
  {
    
  }

  public SuggestionSubModel(SuggestionModel suggestion)
  {
    Id = suggestion.Id;
    Title = suggestion.Title;
  }
}
