List<Transaction> transactionList = new List<Transaction>();

while (true)
{
    Console.WriteLine("\n*Personal Finance Tracker*\n");
    Console.WriteLine("1. Add transaction");
    Console.WriteLine("2. View all transactions");
    Console.WriteLine("3. Delete a transaction");
    Console.WriteLine("4. Exit");

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
        if (!decimal.TryParse(Console.ReadLine(), out decimal purchaseAmount))
        {
            Console.WriteLine("Invalid input\nPress enter to return to the menu");
            Console.ReadLine();
        }

        Transaction transaction = new Transaction();

        transaction.TransactionDescription = purchaseDescription;
        transaction.TransactionDate = purchaseDate;
        transaction.TransactionAmount = purchaseAmount;

        transactionList.Add(transaction);


        SaveData();
    }

    void ViewTransactions()
    {
        Console.WriteLine($"Transaction description\t\t\t\tTransaction Amount\t\tTransaction Date");
        Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------");
        foreach (Transaction transaction in transactionList)
        {
            Console.WriteLine($"{transaction.TransactionDescription, -48}{transaction.TransactionAmount, -32}{transaction.TransactionDate}");
        }

        Console.WriteLine("\nPress enter to return to the menu");
        Console.ReadLine();
    }

    void DeleteTransaction()
    {


        SaveData();
    }

    void SaveData()
    {

    }
}

class Transaction
{
    public string? TransactionDescription { get; set; }
    public string? TransactionDate { get; set; }
    public decimal TransactionAmount { get; set; }
}