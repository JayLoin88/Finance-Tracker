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
                connection.Open();

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
            connection.Open();

            using (SqlCommand command = new SqlCommand(addUserQuery, connection))
            {
                command.Parameters.AddWithValue("@FirstName", FirstName);
                command.Parameters.AddWithValue("@LastName", LastName);
                command.Parameters.AddWithValue("@Balance", Balance);
                command.Parameters.AddWithValue("@MonthlyIncome", MonthlyIncome);
                command.Parameters.AddWithValue("@MonthlyExpenses", MonthlyExpenses);

                int newUserId = (int)command.ExecuteScalar();
                return newUserId;
            }
        }
    }

    public UserSummary? GetUserSummary(int selectedUserId)
    {
        string userSummaryQuery = "SELECT Users.FirstName, Users.LastName, Users.UserId, Users.Balance, Users.MonthlyIncome, Users.MonthlyExpenses, " +
                                                        "COUNT(Transactions.TransactionId) AS TotalTransactions, ISNULL(MAX(Transactions.Amount), 0) AS LargestExpense " +
                                                        "FROM Users " +
                                                        "LEFT OUTER JOIN Transactions ON Transactions.UserId = Users.UserId " +
                                                        "WHERE Users.UserId = @UserId " +
                                                        "GROUP BY Users.FirstName, Users.LastName, Users.UserId, Users.Balance, Users.MonthlyIncome, Users.MonthlyExpenses";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand userSummaryCommand = new SqlCommand(userSummaryQuery, connection))
            {
                userSummaryCommand.Parameters.AddWithValue("@UserId", selectedUserId);

                connection.Open();

                using (SqlDataReader reader = userSummaryCommand.ExecuteReader())
                {
                    if (reader.Read()) // because only one row will be read - used 'if' instead of 'while'
                    {
                        string firstName = reader.GetString(0);
                        string lastName = reader.GetString(1);
                        int userId = reader.GetInt32(2);
                        decimal balance = reader.GetDecimal(3);
                        decimal monthlyIncome = reader.GetDecimal(4);
                        decimal monthlyExpenses = reader.GetDecimal(5);
                        int totalTransactions = reader.GetInt32(6);
                        decimal largestExpense = reader.GetDecimal(7);

                        UserSummary userSummary = new UserSummary();

                        userSummary.FirstName = firstName;
                        userSummary.LastName = lastName;
                        userSummary.UserId = userId;
                        userSummary.Balance = balance;
                        userSummary.MonthlyIncome = monthlyIncome;
                        userSummary.MonthlyExpenses = monthlyExpenses;
                        userSummary.TotalTransactions = totalTransactions;
                        userSummary.LargestExpense = largestExpense;

                        return userSummary;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
    }
}