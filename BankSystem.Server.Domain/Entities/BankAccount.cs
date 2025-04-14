using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Server.Domain.Entities
{
    public class BankAccount : BaseEntity
    {
        public long UserId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public string Currency { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public User User { get; set; }
    }
}
