using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Server.Services.Dtos
{
    public class LoginResponseServiceDto
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public bool TwoFAEnabled { get; set; }
    }
}
