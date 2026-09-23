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
                try
                {
                    connection.Open();
                }
                catch
                {
                    return categoryList;
                }

                using (SqlDataReader reader = loadCategoriesCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int CategoryId = reader.GetInt32(0);
                        string CategoryName = reader.GetString(1);

                        Category category = new Category();

                        category.CategoryId = CategoryId;
                        category.CategoryName = CategoryName;

                        categoryList.Add(category);
                    }
                }
            }
        }

        return categoryList;
    }
}