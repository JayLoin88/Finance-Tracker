using Microsoft.Data.SqlClient;

class UserRepository(string connectionString)
{
    public List<User> GetUsers()
    {
        List<User> userList = new List<User>();

        string loadUsersQuery = "SELECT UserId, FirstName, LastName, Balance, MonthlyIncome, MonthlyExpenses FROM Users";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand loadUsersCommand = new SqlCommand(loadUsersQuery, connection))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Connection Successful");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Connection Failed: {ex.Message}");
                    return userList;
                }

                using (SqlDataReader reader = loadUsersCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int UserId = reader.GetInt32(0);
                        string FirstName = reader.GetString(1);
                        string LastName = reader.GetString(2);
                        decimal Balance = reader.GetDecimal(3);
                        decimal MonthlyIncome = reader.GetDecimal(4);
                        decimal MonthlyExpenses = reader.GetDecimal(5);

                        User user = new User();

                        user.UserId = UserId;
                        user.FirstName = FirstName;
                        user.LastName = LastName;
                        user.Balance = Balance;
                        user.MonthlyIncome = MonthlyIncome;
                        user.MonthlyExpenses = MonthlyExpenses;
                        userList.Add(user);
                    }
                }
            }
        }

        return userList;
    }

    public int AddUser(string FirstName, string LastName, decimal Balance, decimal MonthlyIncome, decimal MonthlyExpenses)
    {
        string addUserQuery = "INSERT INTO Users (FirstName, LastName, Balance, MonthlyIncome, MonthlyExpenses) " +
                                "OUTPUT INSERTED.UserId " +
                                "VALUES (@FirstName, @LastName, @Balance, @MonthlyIncome, @MonthlyExpenses)";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {

            int newUserId;

            try
            {
                connection.Open();
                Console.WriteLine("Connection Successful");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
                newUserId = 0;
                return newUserId;
            }

            using (SqlCommand command = new SqlCommand(addUserQuery, connection))
            {
                command.Parameters.AddWithValue("@FirstName", FirstName);
                command.Parameters.AddWithValue("@LastName", LastName);
                command.Parameters.AddWithValue("@Balance", Balance);
                command.Parameters.AddWithValue("@MonthlyIncome", MonthlyIncome);
                command.Parameters.AddWithValue("@MonthlyExpenses", MonthlyExpenses);

                try
                {
                    newUserId = (int)command.ExecuteScalar();
                    return newUserId;
                }
                catch
                {
                    newUserId = 0;
                    return newUserId;
                }
            }
        }
    }
}