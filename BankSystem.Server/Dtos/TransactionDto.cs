namespace BankSystem.Server.Dtos
{
    public class TransactionDto
    {
        public int UserId { get; set; }
        public string SenderAccountNumber { get; set; } = string.Empty;
        public string ReceiverAccountNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string Details { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
