class UserSummary
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public int UserId { get; set; }
    public decimal Balance { get; set; }
    public decimal MonthlyIncome { get; set; }
    public decimal MonthlyExpenses { get; set; }
    public int TotalTransactions { get; set; }
    public decimal LargestExpense { get; set; }
    public decimal NetSavings => MonthlyIncome - MonthlyExpenses;

}