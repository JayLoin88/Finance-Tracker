using Microsoft.Data.SqlClient;

string connectionString = "Server=localhost;Database=FinanceTracker;Integrated Security=True;TrustServerCertificate=True;";

UserRepository userRepository = new UserRepository(connectionString);
TransactionRepository transactionRepository = new TransactionRepository(connectionString);
CategoryRepository categoryRepository = new CategoryRepository(connectionString);
List<User> userList = userRepository.GetUsers();
List<Category> categoryList = categoryRepository.GetCategories();

while (true)
{
    Console.WriteLine("\n*Personal Finance Tracker*\n");
    Console.WriteLine("1. Add user");
    Console.WriteLine("2. User summary");
    Console.WriteLine("3. Add transaction");
    Console.WriteLine("4. View a users transactions");
    Console.WriteLine("5. Delete a transaction");
    Console.WriteLine("6. Exit\n");

    string? userInput = Console.ReadLine();

    switch (userInput)
    {
        case "1":
            AddUser();
            break;
        case "2":
            UserSummary();
            break;
        case "3":
            AddTransaction();
            break;
        case "4":
            ViewTransactions();
            break;
        case "5":
            DeleteTransaction();
            break;
        case "6":
            return;
        default:
            Console.WriteLine("\nInvalid input\nPress enter to return to the menu");
            Console.ReadLine();
            break;
    }

    void AddUser()
    {
        Console.WriteLine("Please enter the users first name");
        string? firstName = Console.ReadLine();

        if (firstName == null || firstName == "")
        {
            Console.WriteLine("Invalid input\nPress enter to return to the main menu");
            Console.ReadLine();
            return;
        }

        Console.WriteLine("Please enter the users last name");
        string? lastName = Console.ReadLine();

        if (lastName == null || lastName == "")
        {
            Console.WriteLine("Invalid input\nPress enter to return to the main menu");
            Console.ReadLine();
            return;
        }

        Console.WriteLine("Please enter the users current balance");
        if (!decimal.TryParse(Console.ReadLine(), out decimal usersBalance))
        {
            Console.WriteLine("\nInvalid input\nPress enter to return to the main menu");
            Console.ReadLine();
            return;
        }

        Console.WriteLine("Please enter the users monthly income");
        if (!decimal.TryParse(Console.ReadLine(), out decimal usersIncome))
        {
            Console.WriteLine("\nInvalid input\nPress enter to return to the main menu");
            Console.ReadLine();
            return;
        }

        Console.WriteLine("Please enter the users monthly expenses");
        if (!decimal.TryParse(Console.ReadLine(), out decimal usersExpenses))
        {
            Console.WriteLine("\nInvalid input\nPress enter to return to the main menu");
            Console.ReadLine();
            return;
        }

        User user = new User();

        user.FirstName = firstName;
        user.LastName = lastName;
        user.Balance = usersBalance;
        user.MonthlyIncome = usersIncome;
        user.MonthlyExpenses = usersExpenses;

        int newUserId = userRepository.AddUser(user.FirstName, user.LastName, user.Balance, user.MonthlyIncome, user.MonthlyExpenses);

        if (newUserId <= 0)
        {
            Console.WriteLine("Failed to add user\nPress enter to return to the main menu");
            Console.ReadLine();
            return;
        }

        user.UserId = newUserId;
        userList.Add(user);

        Console.WriteLine("\nNew user successfuly added\nPress enter to return to the main menu");
        Console.ReadLine();
    }


    void UserSummary()
    {
        Console.WriteLine("\nPlease select which user you wish to view");

        if (userList.Count > 0)
        {
            DisplayUsers();

            if (int.TryParse(Console.ReadLine(), out int userInput))
            {
                if ((userInput <= userList.Count - 1) && (userInput >= 0))
                {
                    string userSummaryQuery = "SELECT Users.FirstName, Users.LastName, Users.UserId, Users.Balance, Users.MonthlyIncome, Users.MonthlyExpenses, " +
                                                "COUNT(Transactions.TransactionId) AS TotalTransactions, ISNULL(MAX(Transactions.Amount), 0) AS LargestExpense " +
                                                "FROM Users " +
                                                "LEFT OUTER JOIN Transactions ON Transactions.UserId = Users.UserId " +
                                                "WHERE Users.UserId = @UserId " +
                                                "GROUP BY Users.FirstName, Users.LastName, Users.UserId, Users.Balance, Users.MonthlyIncome, Users.MonthlyExpenses";

                    int selectedUserId = userList[userInput].UserId;

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        using (SqlCommand userSummaryCommand = new SqlCommand(userSummaryQuery, connection))
                        {
                            userSummaryCommand.Parameters.AddWithValue("@UserId", selectedUserId);

                            try
                            {
                                connection.Open();
                                Console.WriteLine("Connection successful");

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


                                        decimal NetSavings = monthlyIncome - monthlyExpenses;

                                        Console.WriteLine($"\nUser: {firstName} {lastName}\tUser ID: {userId}");
                                        Console.WriteLine($"Balance: {balance}\n");

                                        Console.WriteLine($"Monthly income: {monthlyIncome}");
                                        Console.WriteLine($"Monthly expenses: {monthlyExpenses}");
                                        Console.WriteLine($"Net savings: {NetSavings}\n");

                                        Console.WriteLine($"Total transactions: {totalTransactions}");
                                        Console.WriteLine($"Largest expense: {largestExpense}\n\n");
                                    }
                                    else
                                    {
                                        Console.WriteLine("User not found or no longer exists. Please reload the application");
                                        Console.WriteLine("Press enter to return to the main menu");
                                        Console.ReadLine();
                                        return;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Operation failure {ex.Message}\nPress enter to return to the main menu");
                                Console.ReadLine();
                                return;
                            }
                        }
                    }

                    Console.WriteLine("Press enter to return to the main menu");
                    Console.ReadLine();
                    return;
                }
            }
            else
            {
                Console.WriteLine("Invalid input\nPress enter to return to the main menu");
                Console.ReadLine();
                return;
            }
        }
        else
        {
            Console.WriteLine("There are no users to view\nPress enter to return to the main menu");
            Console.ReadLine();
        }
    }

    void AddTransaction()
    {
        if (userList.Count > 0)
        {
            Console.WriteLine("Please select which user to add the transaction to");
            DisplayUsers();

            if (int.TryParse(Console.ReadLine(), out int userInput) && (userInput <= userList.Count - 1) && (userInput >= 0))
            {

                int selectedUser = userList[userInput].UserId;

                if (categoryList.Count == 0)
                {
                    Console.WriteLine("There are no categories available\nPress enter to return to the main menu");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine("\nPlease enter the transaction category\n");

                for (int i = 0; i < categoryList.Count; i++)
                {
                    Console.WriteLine($"{i + 1} {categoryList[i].CategoryName}");
                }

                if ((!int.TryParse(Console.ReadLine(), out int categoryChoice)) || categoryChoice < 1 || categoryChoice > categoryList.Count)
                {
                    Console.WriteLine("\nInvalid input\nPress enter to return to the main menu");
                    Console.ReadLine();
                    return;
                }

                int selectedCategory = categoryList[categoryChoice - 1].CategoryId;

                Console.WriteLine("\nPlease enter a description for the transaction");
                string? transactionDescription = Console.ReadLine();

                if (transactionDescription == null)
                {
                    Console.WriteLine("Invalid input\nPress enter to return to the main menu");
                    Console.ReadLine();
                    return;
                }
                else if (transactionDescription == "")
                {
                    transactionDescription = "N/A";
                }

                Console.WriteLine("\nPlease enter the date of the transaction (Format: MM/DD/YYYY)");
                string? purchaseDate = Console.ReadLine();

                string format = "M/d/yyyy";
                bool formatting = DateOnly.TryParseExact(purchaseDate, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateOnly transactionDate);

                if (!formatting)
                {
                    Console.WriteLine("\nInvalid input or format\nPress enter to return to the main menu");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine("\nPlease enter the transaction amount");
                if (!decimal.TryParse(Console.ReadLine(), out decimal purchaseAmount))
                {
                    Console.WriteLine("\nInvalid input\nPress enter to return to the menu");
                    Console.ReadLine();
                    return;
                }

                int rowsAffected = transactionRepository.AddTransaction(purchaseAmount, selectedUser, selectedCategory, transactionDate, transactionDescription);

                if (rowsAffected != 1)
                {
                    Console.WriteLine("Operation failed\nPress enter to return to the main menu");
                    Console.ReadLine();
                    return;
                }
                else
                {
                    Console.WriteLine("Transaction successfully added\nPress enter to return to the main menu");
                    Console.ReadLine();
                    return;
                }

            }
            else
            {
                Console.WriteLine("\nInvalid input\nPress enter to return to the main menu");
                Console.ReadLine();
            }
        }
        else
        {
            Console.WriteLine("\nThere are no users to add a transaction too. Please add a user first"
            + "\nPress enter to return to the main menu");
            Console.ReadLine();
        }
    }

    void ViewTransactions()
    {

        if (!(userList.Count > 0))
        {
            Console.WriteLine("\nThere are no users to add a transaction too. Please add a user first"
            + "\nPress enter to return to the main menu");
            Console.ReadLine();
            return;
        }
        else
        {
            Console.WriteLine("\nPlease select which users transactions to view");

            DisplayUsers();

            if (int.TryParse(Console.ReadLine(), out int userInput) && (userInput <= userList.Count - 1) && (userInput >= 0))
            {
                int selectedUserId = userList[userInput].UserId;

                string viewTransactionsQuery = "SELECT Transactions.Amount, Categories.CategoryName, Transactions.TransactionDate, Transactions.TransactionDescription " +
                                                "FROM Transactions " +
                                                "INNER JOIN Categories " +
                                                "ON Transactions.CategoryId = Categories.CategoryId " +
                                                "WHERE Transactions.UserId = @UserId";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand viewTransactionCommand = new SqlCommand(viewTransactionsQuery, connection))
                    {
                        viewTransactionCommand.Parameters.AddWithValue("@UserId", selectedUserId);

                        try
                        {
                            connection.Open();

                            ViewTransactionHeader();

                            bool transactionsFound = false;

                            using (SqlDataReader reader = viewTransactionCommand.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    decimal amount = reader.GetDecimal(0);
                                    string categoryName = reader.GetString(1);
                                    DateTime fullDateTime = reader.GetDateTime(2);
                                    DateOnly transactionDate = DateOnly.FromDateTime(fullDateTime);
                                    string transactionDescription = reader.GetString(3);

                                    Console.WriteLine($"{categoryName,-32}{transactionDescription,-48}{amount,-32}{transactionDate}");

                                    transactionsFound = true;
                                }

                                if (!transactionsFound)
                                {
                                    Console.WriteLine("There are no transactions saved to this user\nPress enter to return to the main menu");
                                    Console.ReadLine();
                                    return;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Operation failure: {ex.Message}");
                            return;
                        }
                    }
                }

                Console.WriteLine();

                int count = 1;
                foreach (Category category in categoryList)
                {
                    Console.WriteLine($"{count}. {category.CategoryName}");
                    count++;
                }

                Console.WriteLine($"\n{count}. Exit\n");

                if ((!int.TryParse(Console.ReadLine(), out int categoryChoice)) || categoryChoice < 1 || categoryChoice > count)
                {
                    Console.WriteLine("Invalid input\nPress enter to return to the main menu");
                    Console.ReadLine();
                    return;
                }
                else if (categoryChoice == count)
                {
                    return;
                }

                Category selectedFilterCategory = categoryList[categoryChoice - 1];

                FilterTransactions(userInput, selectedFilterCategory.CategoryId);
                return;
            }
            else
            {
                Console.WriteLine("Invalid input\nPress enter to return to the main menu");
                Console.ReadLine();
                return;
            }
        }
    }

    void DeleteTransaction()
    {
        if (userList.Count > 0)
        {
            Console.WriteLine("\nPlease enter which user you wish to delete a transaction from");
            DisplayUsers();
        }
        else
        {
            Console.WriteLine("There are no users to delete a transaction from\nPress enter to return to the main menu");
            Console.ReadLine();
            return;
        }

        if (int.TryParse(Console.ReadLine(), out int userInput))
        {
            if ((userInput <= userList.Count - 1) && (userInput >= 0))
            {
                int selectedUsersId = userList[userInput].UserId;

                string transactionQuery = "SELECT Categories.CategoryName, Transactions.TransactionDescription, Transactions.Amount, Transactions.TransactionDate, Transactions.TransactionId " +
                                            "FROM Transactions " +
                                            "INNER JOIN Categories ON Categories.CategoryId = Transactions.CategoryId " +
                                            "WHERE Transactions.UserId = @UserId";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand transactionCommand = new SqlCommand(transactionQuery, connection))
                    {
                        try
                        {
                            transactionCommand.Parameters.AddWithValue("@UserId", selectedUsersId);

                            connection.Open();

                            ViewTransactionHeader();

                            bool transactionsFound = false;

                            using (SqlDataReader reader = transactionCommand.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string categoryName = reader.GetString(0);
                                    string transactionDescription = reader.GetString(1);
                                    decimal amount = reader.GetDecimal(2);
                                    DateTime fullDate = reader.GetDateTime(3);
                                    DateOnly transactionDate = DateOnly.FromDateTime(fullDate);
                                    int transactionId = reader.GetInt32(4);

                                    Console.WriteLine($"{transactionId}: {categoryName,-29}{transactionDescription,-48}{amount,-32}{transactionDate}");

                                    transactionsFound = true;
                                }

                                if (transactionsFound == false)
                                {
                                    Console.WriteLine("There are no transactions available\nPress enter to return to the main menu");
                                    Console.ReadLine();
                                    return;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Operation failed: {ex.Message}");
                            Console.WriteLine("Press enter to return to the main menu");
                            Console.ReadLine();
                            return;
                        }
                    }
                }

                Console.WriteLine("\nPlease enter the number of the transaction you wish to delete");

                if (!int.TryParse(Console.ReadLine(), out int transactionInput) || !(transactionInput > 0))
                {
                    Console.WriteLine("\nInvalid input\nPress enter to return to the menu");
                    Console.ReadLine();
                    return;
                }

                string deleteTransactionQuery = "DELETE FROM Transactions " +
                                                "WHERE TransactionId = @TransactionId and UserId = @UserId";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        Console.WriteLine("Connection Successful");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Connection failure");
                        return;
                    }

                    using (SqlCommand command = new SqlCommand(deleteTransactionQuery, connection))
                    {
                        command.Parameters.AddWithValue("@TransactionId", transactionInput);
                        command.Parameters.AddWithValue("@UserId", selectedUsersId);

                        try
                        {
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected == 1)
                            {
                                Console.WriteLine("\nTransaction deleted\nPress enter to return to the menu");
                                Console.ReadLine();
                            }
                            else
                            {
                                Console.WriteLine("Transaction failed to delete\nPress enter to return to the main menu.");
                                Console.ReadLine();
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Query failed: {ex.Message}");
                            Console.WriteLine("\nDelete transaction operation failed\nPress enter to return to the main menu");
                            Console.ReadLine();
                            return;

                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("\nInvalid input\nPress enter to return to the menu");
                Console.ReadLine();
                return;
            }
        }
        else
        {
            Console.WriteLine("\nInvalid input\nPress enter to return to the main menu");
            Console.ReadLine();
            return;
        }
    }
}

void DisplayUsers()
{
    for (int i = 0; i < userList.Count; i++)
    {
        Console.WriteLine($"{i}: {userList[i].FirstName} {userList[i].LastName}");
    }
}

void FilterTransactions(int userInput, int categoryId)
{
    string filterTransactionCategory = "SELECT Transactions.Amount, Transactions.TransactionDate, Transactions.TransactionDescription, Categories.CategoryName " +
                                        "From Transactions " +
                                        "INNER JOIN Categories " +
                                        "ON Transactions.CategoryId = Categories.CategoryId " +
                                        "WHERE Transactions.UserId = @UserId AND Transactions.CategoryId = @CategoryId";

    int selectedUserId = userList[userInput].UserId;

    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        using (SqlCommand filterTransactionCommand = new SqlCommand(filterTransactionCategory, connection))
        {
            filterTransactionCommand.Parameters.AddWithValue("@UserId", selectedUserId);
            filterTransactionCommand.Parameters.AddWithValue("@CategoryId", categoryId);

            try
            {
                connection.Open();
                Console.WriteLine("Connection Successful");

                ViewTransactionHeader();

                bool filteredTransactionsFound = false;

                using (SqlDataReader reader = filterTransactionCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        decimal amount = reader.GetDecimal(0);
                        DateTime fullTransactionDate = reader.GetDateTime(1);
                        DateOnly transactionDate = DateOnly.FromDateTime(fullTransactionDate);
                        string transactionDescription = reader.GetString(2);
                        string categoryName = reader.GetString(3);

                        Console.WriteLine($"{categoryName,-32}{transactionDescription,-48}{amount,-32}{transactionDate}");

                        filteredTransactionsFound = true;
                    }

                    if (!filteredTransactionsFound)
                    {
                        Console.WriteLine("There are no transactions under this category saved to this user");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Operation failure: {ex.Message}");
                return;
            }
        }
    }

    Console.WriteLine("\nPress enter to return to the main menu\n");
    Console.ReadLine();
}

void ViewTransactionHeader()
{
    Console.WriteLine($"\nTransaction Category\t\tTransaction Description\t\t\t\tTransaction Amount\t\tTransaction Date");
    Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------");
}

class User
{
    public int UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public decimal Balance { get; set; }
    public decimal MonthlyIncome { get; set; }
    public decimal MonthlyExpenses { get; set; }
}

class Category
{
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
}