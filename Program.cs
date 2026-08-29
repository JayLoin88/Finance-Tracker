using System.Text.Json;
using Microsoft.Data.SqlClient;

List<User> userList = new List<User>();

string connectionString = "Server=localhost;Database=FinanceTracker;Integrated Security=True;TrustServerCertificate=True;";
string startQuery = "SELECT UserId, FirstName, LastName, Balance, MonthlyIncome, MonthlyExpenses FROM Users";
string viewTransactionQuery = "SELECT TransactionId, Amount, UserId, CategoryId, TransactionDate, TransactionDescription FROM Transactions";

/* string fileName = "FinanceTracker.json";
string backupFile = "FinanceTracker.backup.json"; */

using (SqlConnection connection = new SqlConnection(connectionString))
{
    using (SqlCommand command = new SqlCommand(startQuery, connection))
    {
        try
        {
            connection.Open();
            Console.WriteLine("Connection Successful");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Connection Failed: {ex.Message}");
            return;
        }

        using (SqlDataReader reader = command.ExecuteReader())
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

                //Console.WriteLine($"User Id: {UsersId} \n Name: {FirstName} {LastName}");
            }
        }
    }


    using (SqlCommand addTransactions = new SqlCommand(viewTransactionQuery, connection))
    {
        using (SqlDataReader reader = addTransactions.ExecuteReader())
        {
            while (reader.Read())
            {
                int TransactionId = reader.GetInt32(0);
                decimal Amount = reader.GetDecimal(1);
                int UserId = reader.GetInt32(2);
                int CategoryId = reader.GetInt32(3);
                DateTime fullDateTime = reader.GetDateTime(4);
                DateOnly TransactionDate = DateOnly.FromDateTime(fullDateTime);
                string? TransactionDescription = reader.GetString(5);

                Transaction transaction = new Transaction();

                transaction.TransactionId = TransactionId;
                transaction.Amount = Amount;
                transaction.UserId = UserId;
                transaction.CategoryId = CategoryId;
                transaction.TransactionDate = TransactionDate;
                transaction.TransactionDescription = TransactionDescription;

                foreach (User user in userList)
                {
                    if (user.UserId == transaction.UserId)
                    {
                        user.TransactionList.Add(transaction);
                        break;
                    }
                }
            }
        }
    }
    DisplayUsers();
}

/* if (File.Exists(fileName))
{
    var dataFile = File.ReadAllText(fileName);

    try
    {
        userList = JsonSerializer.Deserialize<List<User>>(dataFile) ?? new List<User>();
    }
    catch
    {
        while (true)
        {
            Console.WriteLine("Corrupted or invalid data file. Do you wish to continue? Proceeding will overwrite current save file (FinanceTracker.json)");
            Console.WriteLine("1. yes\n2. No\n");
            string? userInput = Console.ReadLine();

            switch (userInput)
            {
                case "1":
                    File.Copy(fileName, backupFile, true);
                    userList = new List<User>();
                    break;
                case "2":
                    Environment.Exit(0);
                    return;
                default:
                    Console.WriteLine("Invalid option\n");
                    continue;
            }

            break;
        }
    }
} */

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
            //SaveData();
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

        Console.WriteLine("Please enter the users last name");
        string? lastName = Console.ReadLine();

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



        string addUserQuery = "INSERT INTO Users (FirstName, LastName, Balance, MonthlyIncome, MonthlyExpenses) " +
                                "OUTPUT INSERTED.UserId " +
                                "VALUES (@FirstName, @LastName, @Balance, @MonthlyIncome, @MonthlyExpenses)";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {

            try
            {
                connection.Open();
                Console.WriteLine("Connection Successful");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
                return;
            }

            using (SqlCommand command = new SqlCommand(addUserQuery, connection))
            {
                command.Parameters.AddWithValue("@FirstName", firstName);
                command.Parameters.AddWithValue("@LastName", lastName);
                command.Parameters.AddWithValue("@Balance", usersBalance);
                command.Parameters.AddWithValue("@MonthlyIncome", usersIncome);
                command.Parameters.AddWithValue("@MonthlyExpenses", usersExpenses);

                try
                {
                    int newUserId = (int)command.ExecuteScalar();

                    user.UserId = newUserId;
                    userList.Add(user);

                    Console.WriteLine("\nNew user successfuly added\nPress enter to return to the main menu");
                    Console.ReadLine();
                    //SaveData();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Query failed {ex.Message}");
                    Console.WriteLine("\nNew user operation failed\nPress enter to return to the main menu");
                    Console.ReadLine();

                }
            }
        }
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
                    decimal? NetSavings = userList[userInput].MonthlyIncome - userList[userInput].MonthlyExpenses;
                    int TotalTransactions = userList[userInput].TransactionList.Count;

                    decimal LargestExpense = 0m;
                    foreach (var transaction in userList[userInput].TransactionList)
                    {
                        if (transaction.Amount > LargestExpense)
                        {
                            LargestExpense = transaction.Amount;
                        }
                    }

                    Console.WriteLine($"\nUser: {userList[userInput].FirstName} {userList[userInput].LastName}\tUser ID: {userList[userInput].UserId}");
                    Console.WriteLine($"Balance: {userList[userInput].Balance}\n");

                    Console.WriteLine($"Monthly income: {userList[userInput].MonthlyIncome}");
                    Console.WriteLine($"Monthly expenses: {userList[userInput].MonthlyExpenses}");
                    Console.WriteLine($"Net savings: {NetSavings}\n");

                    Console.WriteLine($"Total transactions: {TotalTransactions}");
                    Console.WriteLine($"Largest expense: {LargestExpense}\n\n");


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

                Console.WriteLine("\nPlease enter the transaction category\n");

                string purchaseCategory;
                int numericCategory;

                Console.WriteLine("1. Food and Dining");
                Console.WriteLine("2. Housing and Living");
                Console.WriteLine("3. Health");
                Console.WriteLine("4. Personal Care or Lifestyle");
                Console.WriteLine("5. Financial or Debt");
                Console.WriteLine("6. Other\n");

                string? categoryChoice = Console.ReadLine();

                switch (categoryChoice)
                {
                    case "1":
                        purchaseCategory = "Food and Dining";
                        numericCategory = 1;
                        break;
                    case "2":
                        purchaseCategory = "House and Living";
                        numericCategory = 2;
                        break;
                    case "3":
                        purchaseCategory = "Health";
                        numericCategory = 3;
                        break;
                    case "4":
                        purchaseCategory = "Personal Care or Lifestyle";
                        numericCategory = 4;
                        break;
                    case "5":
                        purchaseCategory = "Financial or Debt";
                        numericCategory = 5;
                        break;
                    case "6":
                        purchaseCategory = "Other";
                        numericCategory = 6;
                        break;
                    default:
                        Console.WriteLine("\nInvalid input\nPress enter to return to the menu");
                        Console.ReadLine();
                        return;
                }

                Console.WriteLine("\nPlease enter a description for the transaction");
                string? transactionDescription = Console.ReadLine();

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

                Transaction transaction = new Transaction();

                //transaction.TransactionCategory = purchaseCategory;
                transaction.Amount = purchaseAmount;
                transaction.UserId = userList[userInput].UserId;
                transaction.CategoryId = numericCategory;
                transaction.TransactionDate = transactionDate;
                transaction.TransactionDescription = transactionDescription;

                string addTransactionQuery = "INSERT INTO Transactions (Amount, UserId, CategoryId, TransactionDate, TransactionDescription) " +
                                            "OUTPUT INSERTED.TransactionId " +
                                            "VALUES (@Amount, @UserId, @CategoryId, @TransactionDate, @TransactionDescription)";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        Console.WriteLine("Connection Successful");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Connection failed: {ex.Message}");
                        return;
                    }

                    using (SqlCommand command = new SqlCommand(addTransactionQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Amount", transaction.Amount);
                        command.Parameters.AddWithValue("@UserId", transaction.UserId);
                        command.Parameters.AddWithValue("@CategoryId", transaction.CategoryId);
                        command.Parameters.AddWithValue("@TransactionDate", transaction.TransactionDate);
                        command.Parameters.AddWithValue("@TransactionDescription", transaction.TransactionDescription);

                        try
                        {
                            int newTransactionId = (int)command.ExecuteScalar();

                            transaction.TransactionId = newTransactionId;
                            userList[userInput].TransactionList.Add(transaction);

                            Console.WriteLine("\nNew transaction successfuly added\nPress enter to return to the main menu");
                            Console.ReadLine();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Query failed {ex.Message}");
                            Console.WriteLine("\nNew transaction operation failed\nPress enter to return to the main menu");
                            Console.ReadLine();
                            return;

                        }
                    }
                }

                //userList[userInput].TransactionList.Add(transaction);

                //SaveData();

                //Console.WriteLine("\nTransaction added\nPress enter to return to the main menu");
                //Console.ReadLine();
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
                if (userList[userInput].TransactionList.Count >= 1)
                {
                    ViewTransactionHeader();


                    foreach (Transaction transaction in userList[userInput].TransactionList)
                    {
                        //fix later
                        Console.WriteLine($"{"temp.TransactionCategoryName",-32}{transaction.TransactionDescription,-48}{transaction.Amount,-32}{transaction.TransactionDate}");
                    }

                    Console.WriteLine("\n\nSort by: \n");
                    Console.WriteLine("1. Food and Dining");
                    Console.WriteLine("2. Housing and Living");
                    Console.WriteLine("3. Health");
                    Console.WriteLine("4. Personal Care or Lifestyle");
                    Console.WriteLine("5. Financial or Debt");
                    Console.WriteLine("6. Other\n");
                    Console.WriteLine("7. Exit\n");


                    int.TryParse(Console.ReadLine(), out int categoryChoice);

                    if (categoryChoice < 7)
                    {
                        FilterTransactions(userInput, categoryChoice);
                        return;
                    }
                    else if (categoryChoice == 7)
                    {
                        return;
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
                    Console.WriteLine("\nThere are no transactions saved\nPress enter to return to the main menu");
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
    }

    void DeleteTransaction()
    {
        if (userList.Count > 0)
        {
            Console.WriteLine("Please enter which user you wish to delete a transaction from");
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
                if (userList[userInput].TransactionList.Count > 0)
                {
                    ViewTransactionHeader();

                    for (int i = 0; i < userList[userInput].TransactionList.Count; i++)
                    {
                        //fix later
                        Console.WriteLine($"{i}: {"temp.TransactionCategoryName",-29}{userList[userInput].TransactionList[i].TransactionDescription,-48}{userList[userInput].TransactionList[i].Amount,-32}{userList[userInput].TransactionList[i].TransactionDate}");
                    }

                    Console.WriteLine("\nPlease enter the number of the transaction you wish to delete");

                    if (!int.TryParse(Console.ReadLine(), out int transactionInput) || !(transactionInput <= userList[userInput].TransactionList.Count - 1) || !(transactionInput >= 0))
                    {
                        Console.WriteLine("\nInvalid input\nPress enter to return to the menu");
                        Console.ReadLine();
                        return;
                    }

                    string deleteTransactionQuery = "DELETE FROM Transactions " +
                                                    "WHERE TransactionId = @TransactionId";

                    int TransactionId = userList[userInput].TransactionList[transactionInput].TransactionId;

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
                            command.Parameters.AddWithValue("@TransactionId", TransactionId);

                            try
                            {
                                int rowsAffected = command.ExecuteNonQuery();

                                if (rowsAffected == 1)
                                {
                                    userList[userInput].TransactionList.RemoveAt(transactionInput);

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
                                Console.WriteLine($"Query failed {ex.Message}");
                                Console.WriteLine("\nDelete transaction operation failed\nPress enter to return to the main menu");
                                Console.ReadLine();
                                return;

                            }
                        }
                    }

                    //SaveData();
                }
                else
                {
                    Console.WriteLine("\nThere are no available transactions\nPress enter to return to the menu");
                    Console.ReadLine();
                    return;
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

/* void SaveData()
{
    string jsonString = JsonSerializer.Serialize(userList, JsonOptions.Options);
    File.WriteAllText(fileName, jsonString);
} */

void FilterTransactions(int userInput, int categoryChoice)
{
    ViewTransactionHeader();
    foreach (Transaction transaction in userList[userInput].TransactionList)
    {
        if (transaction.CategoryId == categoryChoice)
        {
            //fix later
            Console.WriteLine($"{"temp.transactionCategoryName",-32}{transaction.TransactionDescription,-48}{transaction.Amount,-32}{transaction.TransactionDate}");
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

class Transaction
{
    public int TransactionId { get; set; }
    public decimal Amount { get; set; }
    public int UserId { get; set; }
    //public string? TransactionCategory { get; set; }
    public int CategoryId { get; set; }
    public DateOnly TransactionDate { get; set; }
    public string? TransactionDescription { get; set; }
}

class User
{
    public int UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public decimal Balance { get; set; }
    public decimal MonthlyIncome { get; set; }
    public decimal MonthlyExpenses { get; set; }
    public List<Transaction> TransactionList { get; set; } = new List<Transaction>();
}

class JsonOptions
{
    public static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };
}