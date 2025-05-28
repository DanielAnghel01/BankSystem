using AutoMapper;
using BankSystem.Server.Infrastructure.DataAccess;
using BankSystem.Server.Services.Dtos;
using BankSystem.Server.Services.Utils;
using Microsoft.EntityFrameworkCore;
using System.Net;
using BankSystem.Server.Domain.Entities;

namespace BankSystem.Server.Services.Services
{
    public class BankAccountService
    {
        private readonly BankDbContext _bankDbContext;
        private readonly RequestService _requestService;

        public BankAccountService(BankDbContext bankDbContext, RequestService requestService)
        {
            _bankDbContext = bankDbContext;
            _requestService = requestService;
        }

        public async Task<HttpResult> CreateAccountAsync(CreateBankAccountServiceDto createAccountServiceDto)
        {
            if (string.IsNullOrWhiteSpace(createAccountServiceDto.AccountType) ||
                string.IsNullOrWhiteSpace(createAccountServiceDto.Currency) ||
                createAccountServiceDto.Balance <= 0)
            {
                var auditError = new AuditError
                {
                    UserId = createAccountServiceDto.UserId,
                    Action = "CreateBankAccount",
                    Description = "Invalid account creation request: all fields are required and balance must be greater than zero.",
                    Timestamp = DateTime.UtcNow
                };

                await _requestService.SaveAuditError(auditError);

                return HttpResult.Factory.Create(HttpStatusCode.BadRequest, null,
                    "All fields are required and balance must be greater than zero.");
            }

            var account = new BankAccount
            {
                UserId = createAccountServiceDto.UserId,
                AccountNumber = GenerateAccountNumber(),
                AccountType = createAccountServiceDto.AccountType,
                Currency = createAccountServiceDto.Currency,
                Balance = createAccountServiceDto.Balance,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _requestService.SaveBankAccount(account);
            }
            catch (Exception ex)
            {
                var auditError = new AuditError
                {
                    UserId = createAccountServiceDto.UserId,
                    Action = "CreateBankAccount",
                    Description = $"Error saving account: {ex.Message}",
                    Timestamp = DateTime.UtcNow
                };

                await _requestService.SaveAuditError(auditError);

                return HttpResult.Factory.Create(HttpStatusCode.InternalServerError, null,
                    $"Error saving account: {ex.Message}");
            }

            var auditLog = new AuditLog
            {
                UserId = createAccountServiceDto.UserId,
                Action = "CreateBankAccount",
                Timestamp = DateTime.UtcNow,
                Description = $"Created account {account.AccountNumber} of type {account.AccountType} with balance {account.Balance}."
            };

            await _requestService.SaveAuditLog(auditLog);

            return HttpResult.Factory.Create(HttpStatusCode.Created,
                new {
                    message = "Account created successfully",
                    accountNumber = account.AccountNumber,
                    accountType = account.AccountType,
                    currency = account.Currency,
                    balance = account.Balance,
                    createdAt = account.CreatedAt
                });
        }

        public async Task<HttpResult> GetAccountByUser(string userId)
        {
            try
            {
                var bankAccounts = await _bankDbContext.BankAccounts.Where(e => e.UserId.ToString() == userId).ToListAsync();

                if (bankAccounts.Count == 0)
                {
                    var auditError = new AuditError
                    {
                        UserId = Convert.ToInt64(userId),
                        Action = "GetAccountByUser",
                        Description = "No bank accounts found for the user.",
                        Timestamp = DateTime.UtcNow
                    };
                    await _requestService.SaveAuditError(auditError);
                    return HttpResult.Factory.Create(HttpStatusCode.BadRequest, null, "No bank accounts!");
                }
                var auditLog = new AuditLog
                {
                    UserId = Convert.ToInt64(userId),
                    Action = "GetAccountByUser",
                    Timestamp = DateTime.UtcNow,
                    Description = $"Retrieved accounts for user {userId}"
                };
                await _requestService.SaveAuditLog(auditLog);

                return HttpResult.Factory.Create(HttpStatusCode.OK, bankAccounts);
            }
            catch(Exception ex)
            {
                var auditError = new AuditError
                {
                    UserId = Convert.ToInt64(userId),
                    Action = "GetAccountByUser",
                    Description = $"Error retrieving accounts: {ex.Message}",
                    Timestamp = DateTime.UtcNow
                };
                await _requestService.SaveAuditError(auditError);

                return HttpResult.Factory.Create(HttpStatusCode.InternalServerError, null, "Internal server error!");
            }
        }

        // Helper method to generate a random account number
        private string GenerateAccountNumber()
        {
            return "AC" + new Random().Next(100000000, 999999999).ToString();
        }
    }
}
