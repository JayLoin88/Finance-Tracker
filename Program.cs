using System.Text.Json;

List<User> userList = new List<User>();

string fileName = "FinanceTracker.json";
string backupFile = "FinanceTracker.backup.json";


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
        user.NetSavings = usersIncome - usersExpenses;

        userList.Add(user);

        
        Console.WriteLine("\nNew user succesffuly added\nPress enter to return to the main menu");
        Console.ReadLine();
    }

    void UserSummary()
    {
        Console.WriteLine("\nPlease select which user you wish to view");

        if (userList.Count > 0)
        {
            for (int i = 0; i < userList.Count; i++)
            {
                Console.WriteLine($"{i}: {userList[i].UserName}");
            }

            if (int.TryParse(Console.ReadLine(), out int userInput))
            {
                if ((userInput <= userList.Count - 1) && (userInput >= 0))
                {
                    userList[userInput].NetSavings = userList[userInput].MonthlyIncome - userList[userInput].MonthlyExpenses;
                    userList[userInput].totalTransactions = userList[userInput].transactions.Count;

                    Console.WriteLine($"\nUser: {userList[userInput].UserName}");
                    Console.WriteLine($"Balance: {userList[userInput].Balance}\n");

                    Console.WriteLine($"Monthly income: {userList[userInput].MonthlyIncome}");
                    Console.WriteLine($"Monthly expenses: {userList[userInput].MonthlyExpenses}");
                    Console.WriteLine($"Net savings: {userList[userInput].NetSavings}\n");

                    Console.WriteLine($"Total transactions: {userList[userInput].totalTransactions}\n\n");


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
            for (int i = 0; i < userList.Count; i++)
            {
                Console.WriteLine($"{i}: {userList[i].UserName}");
            }

            if (int.TryParse(Console.ReadLine(), out int userInput) && (userInput <= userList.Count - 1))
            {
                Console.WriteLine("\nPlease enter the transaction category");
                string? purchaseCategory = Console.ReadLine();

                Console.WriteLine("\nPlease enter a description for the transaction");
                string? purchaseDescription = Console.ReadLine();

                Console.WriteLine("\nPlease enter the date of the transaction (Format: MM/DD/YYYY)");
                string? purchaseDate = Console.ReadLine();

                Console.WriteLine("\nPlease enter the transaction amount");
                if (decimal.TryParse(Console.ReadLine(), out decimal purchaseAmount))
                {

                    Transaction transaction = new Transaction();

                    transaction.TransactionCategory = purchaseCategory;
                    transaction.TransactionDescription = purchaseDescription;
                    transaction.TransactionDate = purchaseDate;
                    transaction.TransactionAmount = purchaseAmount;

                    userList[userInput].transactions.Add(transaction);

                    SaveData();

                    Console.WriteLine("\nTransaction added\nPress enter to return to the main menu");
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("Invalid input\nPress enter to return to the menu");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("Invalid input\nPress enter to return to the main menu");
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
            Console.WriteLine("Please select which users transactions to view");

            for (int i = 0; i < userList.Count; i++)
            {
                Console.WriteLine($"{i}: {userList[i].UserName}");
            }

            if (int.TryParse(Console.ReadLine(), out int userInput) && (userInput <= userList.Count - 1))
            {
                if (userList[userInput].transactions.Count >= 1)
                {
                    Console.WriteLine($"Transaction Category\t\tTransaction Description\t\t\t\tTransaction Amount\t\tTransaction Date");
                    Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------");

                    foreach (Transaction transaction in userList[userInput].transactions)
                    {
                        Console.WriteLine($"{transaction.TransactionCategory,-32}{transaction.TransactionDescription,-48}{transaction.TransactionAmount,-32}{transaction.TransactionDate}");
                    }

                    Console.WriteLine("\nPress enter to return to the main menu");
                    Console.ReadLine();
                    return;
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
            for (int i = 0; i < userList.Count; i++)
            {
                Console.WriteLine($"{i}: {userList[i].UserName}");
            }
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
                if (userList[userInput].transactions.Count > 0)
                {
                    Console.WriteLine($"Transaction Category\t\tTransaction Description\t\t\t\tTransaction Amount\t\tTransaction Date");
                    Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------");

                    for (int i = 0; i < userList[userInput].transactions.Count; i++)
                    {
                        Console.WriteLine($"{i}: {userList[userInput].transactions[i].TransactionCategory,-29}{userList[userInput].transactions[i].TransactionDescription,-48}{userList[userInput].transactions[i].TransactionAmount,-32}{userList[userInput].transactions[i].TransactionDate}");
                    }

                    Console.WriteLine("\nPlease enter the number of the transaction you wish to delete");

                    if (int.TryParse(Console.ReadLine(), out int transactionInput) && (transactionInput <= userList[userInput].transactions.Count - 1) && (transactionInput >= 0))
                    {
                        userList[userInput].transactions.RemoveAt(transactionInput);
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



    void SaveData()
    {
        for (int i = 0; i < userList.Count; i++)
        {
            userList[i].totalTransactions = userList[i].transactions.Count;
        }
        
        string jsonString = JsonSerializer.Serialize(userList, JsonOptions.Options);
        File.WriteAllText(fileName, jsonString);
    }
}

class Transaction
{
    public string? TransactionCategory { get; set; }
    public string? TransactionDescription { get; set; }
    public string? TransactionDate { get; set; }
    public decimal TransactionAmount { get; set; }
}

class User
{
    public string? UserName { get; set; }
    public decimal Balance { get; set; }
    public decimal MonthlyIncome { get; set; }
    public decimal MonthlyExpenses { get; set; }
    public decimal NetSavings { get; set; }
    public int totalTransactions { get; set; }
    public List<Transaction> transactions { get; set; } = new List<Transaction>();
}

class JsonOptions
{
    public static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };
}