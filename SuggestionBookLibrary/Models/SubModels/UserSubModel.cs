using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuggestionBookLibrary.Models.SubModels;
public class UserSubModel
{
  [BsonRepresentation(BsonType.ObjectId)]
  public string Id { get; set; }
  public string DisplayName { get; set; }

  public UserSubModel()
  {
    
  }

  public UserSubModel(UserModel user)
  {
    Id = user.Id;
    DisplayName = user.DisplayName;
  }
}
