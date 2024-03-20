using Microsoft.Extensions.Caching.Memory;

namespace SuggestionBookLibrary.DataAccess;
public class MongoCategoryData : ICategoryData
{
  private readonly IMongoCollection<CategoryModel> _categories;
  private readonly IMemoryCache _cache;
  private const string CacheName = "CategoryData";

  public MongoCategoryData(INoSqlDbConnection db, IMemoryCache cache)
  {
    _cache = cache;
    _categories = db.CategoryCollection;
  }

  public async Task<List<CategoryModel>> GetAllCategories()
  {
    var output = _cache.Get<List<CategoryModel>>(CacheName);

    if (output == null)
    {
      var results = await _categories.FindAsync(_ => true);
      output = results.ToList();

      _cache.Set(CacheName, output, TimeSpan.FromDays(1));
    }
    return output;
  }

  public async Task CreateCategory(CategoryModel category)
  {
    await _categories.InsertOneAsync(category);
    _cache.Remove(CacheName);
  }

  public async Task UpdateCategory(CategoryModel category)
  {
    var filter = Builders<CategoryModel>.Filter.Eq("Id", category.Id);
    await _categories.ReplaceOneAsync(filter, category);
    _cache.Remove(CacheName);
  }

  public async Task DeleteCategory(string categoryId)
  {
    await _categories.DeleteOneAsync(u => u.Id == categoryId);
    _cache.Remove(CacheName);
  }
}
