class Transaction
{
    public int TransactionId { get; set; }
    public decimal Amount { get; set; }
    public string CategoryName { get; set; } = "";
    public DateOnly TransactionDate { get; set; }
    public string TransactionDescription { get; set; } = "";

}