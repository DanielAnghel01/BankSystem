using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Server.Services.Dtos
{
    public class TransactionHistoryServiceDto
    {
        public string ReciverName { get; set; }
        public string Direction { get; set; }
        public string fromAccountNumber { get; set; }
        public string toAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string description { get; set; }
        public string Currency { get; set; }
    }
}
