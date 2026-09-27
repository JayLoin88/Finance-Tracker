using Microsoft.Data.SqlClient;

string connectionString = "Server=localhost;Database=FinanceTracker;Integrated Security=True;TrustServerCertificate=True;";

UserRepository userRepository = new UserRepository(connectionString);
TransactionRepository transactionRepository = new TransactionRepository(connectionString);
CategoryRepository categoryRepository = new CategoryRepository(connectionString);
List<User> userList = new List<User>();
List<Category> categoryList = new List<Category>();

try
{
    userList = userRepository.GetUsers();
    categoryList = categoryRepository.GetCategories();
}
catch
{
    Console.WriteLine("Unable to access the the database");
    return;
}

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
}

void AddUser()
{
    Console.WriteLine("Please enter the users first name");
    string? firstName = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(firstName))
    {
        Console.WriteLine("Invalid input\nPress enter to return to the main menu");
        Console.ReadLine();
        return;
    }

    firstName = firstName.Trim();

    Console.WriteLine("Please enter the users last name");
    string? lastName = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(lastName))
    {
        Console.WriteLine("Invalid input\nPress enter to return to the main menu");
        Console.ReadLine();
        return;
    }

    lastName = lastName.Trim();

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

    int newUserId;

    try
    {
        newUserId = userRepository.AddUser(user.FirstName, user.LastName, user.Balance, user.MonthlyIncome, user.MonthlyExpenses);
    }
    catch
    {
        Console.WriteLine("Failed to add a new user\nPress enter to return to the main menu");
        Console.ReadLine();
        return;
    }


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

    if (userList.Count == 0)
    {
        Console.WriteLine("There are no users to view\nPress enter to return to the main menu");
        Console.ReadLine();
        return;
    }

    DisplayUsers();

    if (!int.TryParse(Console.ReadLine(), out int userInput))
    {
        Console.WriteLine("Invalid input\nPress enter to return to the main menu");
        Console.ReadLine();
        return;
    }

    if ((userInput >= userList.Count) || (userInput < 0))
    {
        Console.WriteLine("Invalid input\nPress enter to return to the main menu");
        Console.ReadLine();
        return;
    }

    int selectedUserId = userList[userInput].UserId;

    UserSummary? userSummary;

    try
    {
        userSummary = userRepository.GetUserSummary(selectedUserId);
    }
    catch
    {
        Console.WriteLine("Unable to access the database\nPress enter to return to the main menu");
        Console.ReadLine();
        return;
    }

    if (userSummary == null)
    {
        Console.WriteLine("User could not be found\nPress enter to return to the main menu");
        Console.ReadLine();
        return;
    }

    Console.WriteLine($"\nUser: {userSummary.FirstName} {userSummary.LastName}\tUser ID: {userSummary.UserId}");
    Console.WriteLine($"Balance: {userSummary.Balance}\n");

    Console.WriteLine($"Monthly income: {userSummary.MonthlyIncome}");
    Console.WriteLine($"Monthly expenses: {userSummary.MonthlyExpenses}");
    Console.WriteLine($"Net savings: {userSummary.NetSavings}\n");

    Console.WriteLine($"Total transactions: {userSummary.TotalTransactions}");
    Console.WriteLine($"Largest expense: {userSummary.LargestExpense}\n\n");

    Console.WriteLine("Press enter to return to the main menu");
    Console.ReadLine();
}

void AddTransaction()
{
    if (userList.Count == 0)
    {
        Console.WriteLine("\nThere are no users to add a transaction too. Please add a user first"
        + "\nPress enter to return to the main menu");
        Console.ReadLine();
        return;
    }

    Console.WriteLine("Please select which user to add the transaction to");
    DisplayUsers();

    if (!int.TryParse(Console.ReadLine(), out int userInput) || (userInput >= userList.Count) || (userInput < 0))
    {
        Console.WriteLine("\nInvalid input\nPress enter to return to the main menu");
        Console.ReadLine();
        return;
    }

    int selectedUserId = userList[userInput].UserId;

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

    int selectedCategoryId = categoryList[categoryChoice - 1].CategoryId;

    Console.WriteLine("\nPlease enter a description for the transaction");
    string? transactionDescription = Console.ReadLine();

    if (transactionDescription == null)
    {
        Console.WriteLine("Invalid input\nPress enter to return to the main menu");
        Console.ReadLine();
        return;
    }

    transactionDescription = transactionDescription.Trim();

    if (transactionDescription == "")
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

    int rowsAffected;

    try
    {
        rowsAffected = transactionRepository.AddTransaction(purchaseAmount, selectedUserId, selectedCategoryId, transactionDate, transactionDescription);
    }
    catch
    {
        Console.WriteLine("Unable to access the database\nPress enter to return to the main menu");
        Console.ReadLine();
        return;
    }

    if (rowsAffected != 1)
    {
        Console.WriteLine("\nOperation failed\nPress enter to return to the main menu");
        Console.ReadLine();
        return;
    }

    Console.WriteLine("\nTransaction successfully added\nPress enter to return to the main menu");
    Console.ReadLine();
}

void ViewTransactions()
{

    if (userList.Count == 0)
    {
        Console.WriteLine("\nThere are no users to add a transaction to. Please add a user first"
        + "\nPress enter to return to the main menu");
        Console.ReadLine();
        return;
    }

    Console.WriteLine("\nPlease select which users transactions to view");

    DisplayUsers();

    if (!int.TryParse(Console.ReadLine(), out int userInput) || (userInput >= userList.Count) || (userInput < 0))
    {
        Console.WriteLine("Invalid input\nPress enter to return to the main menu");
        Console.ReadLine();
        return;
    }

    int selectedUserId = userList[userInput].UserId;

    List<Transaction> transactionList;

    try
    {
        transactionList = transactionRepository.GetTransactions(selectedUserId);
    }
    catch
    {
        Console.WriteLine("Unable to access the database\nPress enter to return to the main menu");
        Console.ReadLine();
        return;
    }

    if (transactionList.Count == 0)
    {
        Console.WriteLine("There are no transactions for this user\nPress enter to return to the main menu");
        Console.ReadLine();
        return;
    }

    ViewTransactionHeader();

    foreach (Transaction transaction in transactionList)
    {
        Console.WriteLine($"{transaction.CategoryName,-32}{transaction.TransactionDescription,-48}{transaction.Amount,-32}{transaction.TransactionDate}");
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

    if (categoryChoice == count)
    {
        return;
    }

    Category selectedFilterCategory = categoryList[categoryChoice - 1];

    FilterTransactions(selectedUserId, selectedFilterCategory.CategoryId);
}

void DeleteTransaction()
{
    if (userList.Count == 0)
    {
        Console.WriteLine("There are no users to delete a transaction from\nPress enter to return to the main menu");
        Console.ReadLine();
        return;
    }

    Console.WriteLine("\nPlease enter which user you wish to delete a transaction from");
    DisplayUsers();

    if (!int.TryParse(Console.ReadLine(), out int userInput))
    {
        Console.WriteLine("\nInvalid input\nPress enter to return to the main menu");
        Console.ReadLine();
        return;
    }

    if ((userInput >= userList.Count) || (userInput < 0))
    {
        Console.WriteLine("\nInvalid input\nPress enter to return to the menu");
        Console.ReadLine();
        return;
    }

    int selectedUserId = userList[userInput].UserId;

    List<Transaction> transactionList;

    try
    {
        transactionList = transactionRepository.GetTransactions(selectedUserId);
    }
    catch
    {
        Console.WriteLine("Failed to access the database\nPress enter to return to the main menu");
        Console.ReadLine();
        return;
    }

    if (transactionList.Count == 0)
    {
        Console.WriteLine("This user does not have any transactions\nPress enter to return to the main menu");
        Console.ReadLine();
        return;
    }

    ViewTransactionHeader();
    foreach (Transaction transaction in transactionList)
    {
        Console.WriteLine($"{transaction.TransactionId}: {transaction.CategoryName,-29}{transaction.TransactionDescription,-48}{transaction.Amount,-32}{transaction.TransactionDate}");
    }

    Console.WriteLine("\nPlease enter the number of the transaction you wish to delete");

    if (!int.TryParse(Console.ReadLine(), out int transactionInput) || (transactionInput <= 0))
    {
        Console.WriteLine("\nInvalid input\nPress enter to return to the menu");
        Console.ReadLine();
        return;
    }

    int rowsAffected;

    try
    {
        rowsAffected = transactionRepository.DeleteTransaction(selectedUserId, transactionInput);
    }
    catch
    {
        Console.WriteLine("Failed to access the database\nPress enter to return to the main menu");
        Console.ReadLine();
        return;
    }

    if (rowsAffected != 1)
    {
        Console.WriteLine("Operation failed\nPress enter to return to the main menu");
        Console.ReadLine();
        return;
    }

    Console.WriteLine("\nTransaction successfully deleted\nPress enter to return to the main menu");
    Console.ReadLine();
}

void DisplayUsers()
{
    for (int i = 0; i < userList.Count; i++)
    {
        Console.WriteLine($"{i}: {userList[i].FirstName} {userList[i].LastName}");
    }
}

void FilterTransactions(int userId, int categoryId)
{

    List<Transaction> filteredTransactions;

    try
    {
        filteredTransactions = transactionRepository.FilterTransactions(userId, categoryId);
    }
    catch
    {
        Console.WriteLine("Unable to access the database\nPress enter to return to the main menu");
        Console.ReadLine();
        return;
    }

    if (filteredTransactions.Count == 0)
    {
        Console.WriteLine("There are no transactions in this category for this user\nPress enter to return to the main menu");
        Console.ReadLine();
        return;
    }

    ViewTransactionHeader();

    foreach (Transaction transaction in filteredTransactions)
    {
        Console.WriteLine($"{transaction.CategoryName,-32}{transaction.TransactionDescription,-48}{transaction.Amount,-32}{transaction.TransactionDate}");
    }

    Console.WriteLine("Press enter to return to the main menu");
    Console.ReadLine();
}

void ViewTransactionHeader()
{
    Console.WriteLine($"\nTransaction Category\t\tTransaction Description\t\t\t\tTransaction Amount\t\tTransaction Date");
    Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------");
}