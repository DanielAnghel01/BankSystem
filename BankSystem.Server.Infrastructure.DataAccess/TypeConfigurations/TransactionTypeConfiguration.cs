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
    public class TransactionTypeConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("TRANSACTION");
            builder.HasKey(x => x.Id);
            builder.HasOne(a => a.SenderAccount).WithMany().HasForeignKey(x => x.SenderAccountId);
            builder.HasOne(a => a.ReciverAccount).WithMany().HasForeignKey(x => x.ReciverAccountId);
        }
    }
}
