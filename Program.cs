using System.Text.Json;

List<Transaction> transactionList = new List<Transaction>();
List<User> userList = new List<User>();

string fileName = "FinanceTracker.json";
string backupFile = "FinanceTracker.backup.json";


if (File.Exists(fileName))
{
    var dataFile = File.ReadAllText(fileName);

    try
    {
        transactionList = JsonSerializer.Deserialize<List<Transaction>>(dataFile) ?? new List<Transaction>();
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
                    transactionList = new List<Transaction>();
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
            Console.WriteLine("Invalid input");
            break;
    }

    void AddUser()
    {
        Console.WriteLine("Please enter the users name");
        string? usersName = Console.ReadLine();

        Console.WriteLine("Please enter the users current balance");
        if (!decimal.TryParse(Console.ReadLine(), out decimal usersBalance))
        {
            Console.WriteLine("Invalid input\nPress enter to return to the main menu");
            Console.ReadLine();
            return; // may havge to reverse the order of if statements
        }

        Console.WriteLine("Please enter the users monthly income");
        if (!decimal.TryParse(Console.ReadLine(), out decimal usersIncome))
        {
            Console.WriteLine("Invalid input\nPress enter to return to the main menu");
            Console.ReadLine();
        }

        Console.WriteLine("Please enter the users monthly expenses");
        if (!decimal.TryParse(Console.ReadLine(), out decimal usersExpenses))
        {
            Console.WriteLine("Invalid input\nPress enter to return to the main menu");
            Console.ReadLine();
        }

        User user = new User();

        user.userName = usersName;
        user.Balance = usersBalance;
        user.MonthlyIncome = usersIncome;
        user.MonthlyExpenses = usersExpenses;

        userList.Add(user);

    }

    void UserSummary()
    {
        while (true)
        {
            //Console.WriteLine($"Current Balance: {User.Balance}");
        }
    }

    void AddTransaction()
    {
        Console.WriteLine("\nPlease enter a description for the transaction");
        string? purchaseDescription = Console.ReadLine();

        Console.WriteLine("\nPlease enter the date of the transaction (Format: MM/DD/YYYY)");
        string? purchaseDate = Console.ReadLine();

        Console.WriteLine("\nPlease enter the transaction amount");
        if (decimal.TryParse(Console.ReadLine(), out decimal purchaseAmount))
        {
            Transaction transaction = new Transaction();

            transaction.TransactionDescription = purchaseDescription;
            transaction.TransactionDate = purchaseDate;
            transaction.TransactionAmount = purchaseAmount;

            transactionList.Add(transaction);

            SaveData();
        }
        else
        {
            Console.WriteLine("Invalid input\nPress enter to return to the menu");
            Console.ReadLine();
        }


    }

    void ViewTransactions()
    {
        Console.WriteLine($"Transaction description\t\t\t\tTransaction Amount\t\tTransaction Date");
        Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------");
        foreach (Transaction transaction in transactionList)
        {
            Console.WriteLine($"{transaction.TransactionDescription,-48}{transaction.TransactionAmount,-32}{transaction.TransactionDate}");
        }

        Console.WriteLine("\nPress enter to return to the menu");
        Console.ReadLine();
    }

    void DeleteTransaction()
    {
        if (transactionList.Count > 0)
        {
            Console.WriteLine($"Transaction description\t\t\t\tTransaction Amount\t\tTransaction Date");
            Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------");

            for (int i = 0; i < transactionList.Count; i++)
            {
                Console.WriteLine($"{i}: {transactionList[i].TransactionDescription,-48}{transactionList[i].TransactionAmount,-32}{transactionList[i].TransactionDate}");
            }

            Console.WriteLine("\nPlease enter the number of the transaction you wish to delete");

            if (int.TryParse(Console.ReadLine(), out int userInput))
            {
                transactionList.RemoveAt(userInput);
            }

            SaveData();

            Console.WriteLine("\nTransaction deleted\nPress enter to return to the menu");
            Console.ReadLine();
        }
        else
        {
            Console.WriteLine("There are no available transactions\nPress enter to return to the menu");
            Console.ReadLine();
        }
    }

    void SaveData()
    {
        string jsonString = JsonSerializer.Serialize(transactionList, JsonOptions.Options);
        File.WriteAllText(fileName, jsonString);
    }
}

class Transaction
{
    public string? TransactionDescription { get; set; }
    public string? TransactionDate { get; set; }
    public decimal TransactionAmount { get; set; }
}

class User
{
    public string? userName;
    public decimal Balance { get; set; }
    public decimal MonthlyIncome { get; set; }
    public decimal MonthlyExpenses { get; set; }
    public decimal NetSavings { get; set; }
}

class JsonOptions
{
    public static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };
}