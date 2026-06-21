using System.Text.Json;

List<Transaction> transactionList = new List<Transaction>();
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
    Console.WriteLine("1. Add transaction");
    Console.WriteLine("2. Add transaction");
    Console.WriteLine("3. View all transactions");
    Console.WriteLine("4. Delete a transaction");
    Console.WriteLine("5. Exit\n");

    string? userInput = Console.ReadLine();

    switch (userInput)
    {
        case "1":
            AddTransaction();
            break;
        case "2":
            ViewTransactions();
            break;
        case "3":
            DeleteTransaction();
            break;
        case "4":
            SaveData();
            return;
        default:
            Console.WriteLine("Invalid input");
            break;
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

class JsonOptions
{
    public static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };
}