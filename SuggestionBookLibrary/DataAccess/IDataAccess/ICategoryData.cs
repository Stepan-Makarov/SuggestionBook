namespace SuggestionBookLibrary.DataAccess.IDataAccess;

public interface ICategoryData
{
  Task CreateCategory(CategoryModel category);
  Task DeleteCategory(string categoryId);
  Task<List<CategoryModel>> GetAllCategories();
  Task UpdateCategory(CategoryModel category);
}