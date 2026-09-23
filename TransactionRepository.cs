using Microsoft.Data.SqlClient;

class TransactionRepository(string connectionString)
{
    public int AddTransaction(decimal purchaseAmount, int selectedUser, int selectedCategory, DateOnly transactionDate, string transactionDescription)
    {
        string addTransactionQuery = "INSERT INTO Transactions (Amount, UserId, CategoryId, TransactionDate, TransactionDescription) " +
                                            "VALUES (@Amount, @UserId, @CategoryId, @TransactionDate, @TransactionDescription)";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();
            }
            catch
            {
                return 0;
            }

            using (SqlCommand command = new SqlCommand(addTransactionQuery, connection))
            {
                command.Parameters.AddWithValue("@Amount", purchaseAmount);
                command.Parameters.AddWithValue("@UserId", selectedUser);
                command.Parameters.AddWithValue("@CategoryId", selectedCategory);
                command.Parameters.AddWithValue("@TransactionDate", transactionDate);
                command.Parameters.AddWithValue("@TransactionDescription", transactionDescription);

                try
                {
                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected;
                }
                catch
                {
                    return 0;
                }
            }
        }
    }
}