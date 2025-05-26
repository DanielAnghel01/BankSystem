using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Server.Services.Dtos
{
    public class DepositServiceDto
    {
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string UserId { get; set; }
    }
}
