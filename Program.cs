using System.Text.Json;
using Microsoft.Data.SqlClient;

List<User> userList = new List<User>();

string connectionString = "Server=localhost;Database=FinanceTracker;Integrated Security=True;TrustServerCertificate=True;";
string query = "SELECT FirstName, LastName FROM Users";

string fileName = "FinanceTracker.json";
string backupFile = "FinanceTracker.backup.json";

using (SqlConnection connection = new SqlConnection(connectionString))
{
    using (SqlCommand command = new SqlCommand(query, connection))
    {
        try
        {
            connection.Open();
            Console.WriteLine("Connection Succesful");
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
                string FirstName = reader.GetString(0);
                string LastName = reader.GetString(1);

                Console.WriteLine($"Name: {FirstName} {LastName}");
            }
        }
    }
}

if (File.Exists(fileName))
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
}

while (true)
{
    Console.WriteLine("\n*Personal Finance Tracker*\n");
    Console.WriteLine("1. Add user");
    Console.WriteLine("2. User summary");
    Console.WriteLine("3. Add transaction");
    Console.WriteLine("4. View all transactions");
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
            SaveData();
            return;
        default:
            Console.WriteLine("\nInvalid input\nPress enter to return to the menu");
            Console.ReadLine();
            break;
    }

    void AddUser()
    {
        Console.WriteLine("Please enter the users name");
        string? usersName = Console.ReadLine();

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

        user.UserName = usersName;
        user.Balance = usersBalance;
        user.MonthlyIncome = usersIncome;
        user.MonthlyExpenses = usersExpenses;

        userList.Add(user);
        SaveData();

        Console.WriteLine("\nNew user succesffuly added\nPress enter to return to the main menu");
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
                    decimal NetSavings = userList[userInput].MonthlyIncome - userList[userInput].MonthlyExpenses;
                    int TotalTransactions = userList[userInput].Transactions.Count;

                    decimal LargestExpense = 0m;
                    foreach (var transaction in userList[userInput].Transactions)
                    {
                        if (transaction.TransactionAmount > LargestExpense)
                        {
                            LargestExpense = transaction.TransactionAmount;
                        }
                    }

                    Console.WriteLine($"\nUser: {userList[userInput].UserName}");
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
                string? purchaseDescription = Console.ReadLine();

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
                if (decimal.TryParse(Console.ReadLine(), out decimal purchaseAmount))
                {

                    Transaction transaction = new Transaction();

                    transaction.TransactionCategory = purchaseCategory;
                    transaction.NumericCategory = numericCategory;
                    transaction.TransactionDescription = purchaseDescription;
                    transaction.TransactionDate = transactionDate;
                    transaction.TransactionAmount = purchaseAmount;

                    userList[userInput].Transactions.Add(transaction);

                    SaveData();

                    Console.WriteLine("\nTransaction added\nPress enter to return to the main menu");
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("\nInvalid input\nPress enter to return to the menu");
                    Console.ReadLine();
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
                if (userList[userInput].Transactions.Count >= 1)
                {
                    ViewTransactionHeader();

                    foreach (Transaction transaction in userList[userInput].Transactions)
                    {
                        Console.WriteLine($"{transaction.TransactionCategory,-32}{transaction.TransactionDescription,-48}{transaction.TransactionAmount,-32}{transaction.TransactionDate}");
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
            if (userList[userInput].Transactions.Count > 0)
            {
                ViewTransactionHeader();

                for (int i = 0; i < userList[userInput].Transactions.Count; i++)
                {
                    Console.WriteLine($"{i}: {userList[userInput].Transactions[i].TransactionCategory,-29}{userList[userInput].Transactions[i].TransactionDescription,-48}{userList[userInput].Transactions[i].TransactionAmount,-32}{userList[userInput].Transactions[i].TransactionDate}");
                }

                Console.WriteLine("\nPlease enter the number of the transaction you wish to delete");

                if (int.TryParse(Console.ReadLine(), out int transactionInput) && (transactionInput <= userList[userInput].Transactions.Count - 1) && (transactionInput >= 0))
                {
                    userList[userInput].Transactions.RemoveAt(transactionInput);
                }
                else
                {
                    Console.WriteLine("\nInvalid input\nPress enter to return to the menu");
                    Console.ReadLine();
                    return;
                }

                SaveData();

                Console.WriteLine("\nTransaction deleted\nPress enter to return to the menu");
                Console.ReadLine();
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

void DisplayUsers()
{
    for (int i = 0; i < userList.Count; i++)
    {
        Console.WriteLine($"{i}: {userList[i].UserName}");
    }
}

void SaveData()
{
    string jsonString = JsonSerializer.Serialize(userList, JsonOptions.Options);
    File.WriteAllText(fileName, jsonString);
}

void FilterTransactions(int userInput, int categoryChoice)
{
    ViewTransactionHeader();
    foreach (Transaction transaction in userList[userInput].Transactions)
    {
        if (transaction.NumericCategory == categoryChoice)
        {
            Console.WriteLine($"{transaction.TransactionCategory,-32}{transaction.TransactionDescription,-48}{transaction.TransactionAmount,-32}{transaction.TransactionDate}");
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
    public string? TransactionCategory { get; set; }
    public int NumericCategory { get; set; }
    public string? TransactionDescription { get; set; }
    public DateOnly TransactionDate { get; set; }
    public decimal TransactionAmount { get; set; }
}

class User
{
    public string? UserName { get; set; }
    public decimal Balance { get; set; }
    public decimal MonthlyIncome { get; set; }
    public decimal MonthlyExpenses { get; set; }
    public List<Transaction> Transactions { get; set; } = new List<Transaction>();
}

class JsonOptions
{
    public static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };
}