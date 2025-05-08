using BankSystem.Server.Domain.Entities;
using BankSystem.Server.Infrastructure.DataAccess.TypeConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BankSystem.Server.Infrastructure.DataAccess
{
    public class BankDbContext : IdentityDbContext<IdentityUser>
    {
        public BankDbContext(DbContextOptions<BankDbContext> options) : base(options)
        {
        }

        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<LoginToken> LoginTokens { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUser>(b => { b.ToTable("USER", "BANK"); });
            modelBuilder.Entity<IdentityRole>(b => { b.ToTable("AspNetRoles", "BANK"); });
            modelBuilder.Entity<IdentityUserRole<string>>(b => { b.ToTable("AspNetUserRoles", "BANK"); });
            modelBuilder.Entity<IdentityUserClaim<string>>(b => { b.ToTable("AspNetUserClaims", "BANK"); });
            modelBuilder.Entity<IdentityUserLogin<string>>(b => { b.ToTable("AspNetUserLogins", "BANK"); });
            modelBuilder.Entity<IdentityRoleClaim<string>>(b => { b.ToTable("AspNetRoleClaims", "BANK"); });
            modelBuilder.Entity<IdentityUserToken<string>>(b => { b.ToTable("AspNetUserTokens", "BANK"); });

            modelBuilder.HasDefaultSchema("BANK");
            modelBuilder.ApplyConfiguration(new AuditLogTypeConfiguration());
            modelBuilder.ApplyConfiguration(new BankAccountTypeConfiguration());
            modelBuilder.ApplyConfiguration(new LoginTokenTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserTypeConfiguration());
        }
    }
}
