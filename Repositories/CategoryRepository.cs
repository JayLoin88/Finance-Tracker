using Microsoft.Data.SqlClient;

class CategoryRepository(string connectionString)
{
    public List<Category> GetCategories()
    {

        List<Category> categoryList = new List<Category>();

        string loadCategoriesQuery = "SELECT CategoryId, CategoryName FROM Categories ORDER BY CategoryId";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand loadCategoriesCommand = new SqlCommand(loadCategoriesQuery, connection))
            {
                connection.Open();

                using (SqlDataReader reader = loadCategoriesCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int categoryId = reader.GetInt32(0);
                        string categoryName = reader.GetString(1);

                        Category category = new Category();

                        category.CategoryId = categoryId;
                        category.CategoryName = categoryName;

                        categoryList.Add(category);
                    }
                }
            }
        }
        return categoryList;
    }
}