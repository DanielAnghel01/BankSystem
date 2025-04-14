using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Server.Domain.Entities
{
    public class Transaction : BaseEntity
    {
        public long SenderAccountId { get; set; }
        public long ReciverAccountId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public DateTime Date { get; set; }
        public string Details { get; set; }

        public BankAccount SenderAccount { get; set; }
        public BankAccount ReciverAccount { get; set; }
    }
}
