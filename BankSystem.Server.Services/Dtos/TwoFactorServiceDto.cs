using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Server.Services.Dtos
{
    public class TwoFactorServiceDto
    {
        public string Username { get; set; }
        public string Code { get; set; }
    }
}
