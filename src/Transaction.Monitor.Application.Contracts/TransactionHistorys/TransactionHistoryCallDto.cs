namespace Transaction.Monitor.TransactionHistorys;

public class TransactionHistoryCallDto
{
    public string Tx { get; set; }
    public string ToAddress { get; set; }
    public string FromAddress { get; set; }
    public string Symbol { get; set; }
    public string Amount { get; set; }
}