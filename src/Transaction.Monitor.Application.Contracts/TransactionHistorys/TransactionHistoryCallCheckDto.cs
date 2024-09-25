namespace Transaction.Monitor.TransactionHistorys;

public class TransactionHistoryCallCheckDto
{
    public string Tx { get; set; }
    public string ToAddress { get; set; }
    public string FromAddress { get; set; }
    public string Symbol { get; set; }
    public string Amount { get; set; }
    public string Memo { get; set; }
    public string Key { get; set; }
    public string Signature { get; set; }
    
}