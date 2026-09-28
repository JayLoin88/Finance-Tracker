using Microsoft.Data.SqlClient;

class TransactionRepository(string connectionString)
{
    public int AddTransaction(decimal purchaseAmount, int userId, int categoryId, DateOnly transactionDate, string transactionDescription)
    {
        string addTransactionQuery = "INSERT INTO Transactions (Amount, UserId, CategoryId, TransactionDate, TransactionDescription) " +
                                            "VALUES (@Amount, @UserId, @CategoryId, @TransactionDate, @TransactionDescription)";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand addTransactionCommand = new SqlCommand(addTransactionQuery, connection))
            {
                addTransactionCommand.Parameters.AddWithValue("@Amount", purchaseAmount);
                addTransactionCommand.Parameters.AddWithValue("@UserId", userId);
                addTransactionCommand.Parameters.AddWithValue("@CategoryId", categoryId);
                addTransactionCommand.Parameters.AddWithValue("@TransactionDate", transactionDate);
                addTransactionCommand.Parameters.AddWithValue("@TransactionDescription", transactionDescription);

                int rowsAffected = addTransactionCommand.ExecuteNonQuery();

                return rowsAffected;
            }
        }
    }

    public List<Transaction> GetTransactions(int userId)
    {
        List<Transaction> transactionList = new List<Transaction>();

        string viewTransactionsQuery = "SELECT Transactions.TransactionId, Transactions.Amount, Categories.CategoryName, Transactions.TransactionDate, Transactions.TransactionDescription " +
                                                "FROM Transactions " +
                                                "INNER JOIN Categories " +
                                                "ON Transactions.CategoryId = Categories.CategoryId " +
                                                "WHERE Transactions.UserId = @UserId";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand getTransactionsCommand = new SqlCommand(viewTransactionsQuery, connection))
            {
                getTransactionsCommand.Parameters.AddWithValue("@UserId", userId);

                using (SqlDataReader reader = getTransactionsCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int transactionId = reader.GetInt32(0);
                        decimal amount = reader.GetDecimal(1);
                        string categoryName = reader.GetString(2);
                        DateTime fullTransactionDate = reader.GetDateTime(3);
                        DateOnly transactionDate = DateOnly.FromDateTime(fullTransactionDate);
                        string transactionDescription = reader.GetString(4);

                        Transaction transaction = new Transaction();

                        transaction.TransactionId = transactionId;
                        transaction.Amount = amount;
                        transaction.CategoryName = categoryName;
                        transaction.TransactionDate = transactionDate;
                        transaction.TransactionDescription = transactionDescription;

                        transactionList.Add(transaction);
                    }
                }
            }
        }
        return transactionList;
    }

    public List<Transaction> FilterTransactions(int userId, int categoryId)
    {
        List<Transaction> filteredTransactions = new List<Transaction>();

        string filterTransactionCategory = "SELECT Transactions.Amount, Transactions.TransactionDate, Transactions.TransactionDescription, Categories.CategoryName " +
                                        "FROM Transactions " +
                                        "INNER JOIN Categories " +
                                        "ON Transactions.CategoryId = Categories.CategoryId " +
                                        "WHERE Transactions.UserId = @UserId AND Transactions.CategoryId = @CategoryId";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {

            using (SqlCommand filterTransactionCommand = new SqlCommand(filterTransactionCategory, connection))
            {
                filterTransactionCommand.Parameters.AddWithValue("@UserId", userId);
                filterTransactionCommand.Parameters.AddWithValue("@CategoryId", categoryId);

                connection.Open();

                using (SqlDataReader reader = filterTransactionCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        decimal amount = reader.GetDecimal(0);
                        DateTime fullTransactionDate = reader.GetDateTime(1);
                        DateOnly transactionDate = DateOnly.FromDateTime(fullTransactionDate);
                        string transactionDescription = reader.GetString(2);
                        string categoryName = reader.GetString(3);

                        Transaction transaction = new Transaction();

                        transaction.Amount = amount;
                        transaction.TransactionDate = transactionDate;
                        transaction.TransactionDescription = transactionDescription;
                        transaction.CategoryName = categoryName;

                        filteredTransactions.Add(transaction);

                    }
                }
                return filteredTransactions;
            }
        }
    }

    public int DeleteTransaction(int userId, int transactionId)
    {
        string deleteTransactionQuery = "DELETE FROM Transactions " +
                                                "WHERE TransactionId = @TransactionId AND UserId = @UserId";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand(deleteTransactionQuery, connection))
            {
                command.Parameters.AddWithValue("@TransactionId", transactionId);
                command.Parameters.AddWithValue("@UserId", userId);

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected;
            }
        }
    }
}