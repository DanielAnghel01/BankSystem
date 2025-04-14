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
    public class LoginTokenTypeConfiguration : IEntityTypeConfiguration<LoginToken>
    {
        public void Configure(EntityTypeBuilder<LoginToken> builder)
        {
            builder.ToTable("LOGIN_TOKEN");
            builder.HasKey(x => x.Id);
            builder.HasOne(a => a.User).WithMany().HasForeignKey(x => x.UserId);
        }
    }
}
