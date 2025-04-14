using BankSystem.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Server.Infrastructure.DataAccess.TypeConfigurations
{
    public class AuditLogTypeConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AUDIT_LOGS");
            builder.HasKey(x => x.Id);
            builder.HasOne(a => a.User).WithMany().HasForeignKey(x => x.UserId);
        }
    }
}
