using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Server.Domain.Entities
{
    public class LoginToken : BaseEntity
    {
        public long UserId { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime RevokedAt { get; set; }

        public User User { get; set; }
    }
}
