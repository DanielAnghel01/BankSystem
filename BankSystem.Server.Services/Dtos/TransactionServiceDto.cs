using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Server.Services.Dtos
{
    public class TransactionServiceDto
    {
        public string SenderAccountNumber { get; set; }
        public string ReciverAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string Details { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
