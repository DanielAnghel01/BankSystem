using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankSystem.Server.Domain.Entities;

namespace BankSystem.Server.Services.Dtos
{
    public class UserProfileServiceDto
    {
        public User User { get; set; }
        public List<BankAccount> BankAccounts { get; set; }
    }
}
