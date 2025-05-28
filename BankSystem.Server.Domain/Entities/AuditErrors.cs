using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Server.Domain.Entities
{
    public class AuditError : BaseEntity
    {
        public long? UserId { get; set; }
        public string Action { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }

        public User User { get; set; }
    }
}
