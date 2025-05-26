namespace BankSystem.Server.Dtos
{
    public class TransactionDto
    {
        public string SenderAccountNumber { get; set; }
        public string ReciverAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string Details { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
