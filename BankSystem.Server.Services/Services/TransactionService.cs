﻿using BankSystem.Server.Infrastructure.DataAccess;
using BankSystem.Server.Services.Dtos;
using BankSystem.Server.Services.Utils;
using BankSystem.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Text.Json.Serialization;
using System.Text.Json;
using Newtonsoft.Json;

namespace BankSystem.Server.Services.Services
{
    public class TransactionService
    {
        private readonly BankDbContext _bankRepository;
        private readonly RequestService _requestService;

        public TransactionService(BankDbContext bankRepository, RequestService requestService)
        {
            _bankRepository = bankRepository;
            _requestService = requestService;
        }

        public async Task<HttpResult> TransferAsync(TransactionServiceDto transactionServiceDto)
        {
            if (transactionServiceDto.Amount <= 0)
            {
                var auditError = new AuditError
                {
                    UserId = null,
                    Action = "Transfer",
                    Description = "Amount must be greater than zero.",
                    Timestamp = DateTime.UtcNow
                };
                await _requestService.SaveAuditError(auditError);

                return HttpResult.Factory.Create(HttpStatusCode.BadRequest, null, "Amount must be greater than zero.");
            }

            var sender = await _bankRepository.BankAccounts
                .FirstOrDefaultAsync(a => a.AccountNumber == transactionServiceDto.SenderAccountNumber);
            var receiver = await _bankRepository.BankAccounts
                .FirstOrDefaultAsync(a => a.AccountNumber == transactionServiceDto.ReciverAccountNumber);

            if (sender == null || receiver == null)
            {
                var auditError = new AuditError
                {
                    UserId = sender?.UserId ?? receiver?.UserId,
                    Action = "Transfer",
                    Description = "Sender or receiver account not found.",
                    Timestamp = DateTime.UtcNow
                };
                await _requestService.SaveAuditError(auditError);

                return HttpResult.Factory.Create(HttpStatusCode.NotFound, null, "Sender or receiver account not found.");
            }

            if (sender.Balance < transactionServiceDto.Amount)
            {
                var auditError = new AuditError
                {
                    UserId = sender.UserId,
                    Action = "Transfer",
                    Description = "Insufficient balance in sender account.",
                    Timestamp = DateTime.UtcNow
                };
                await _requestService.SaveAuditError(auditError);

                return HttpResult.Factory.Create(HttpStatusCode.BadRequest, null, "Insufficient balance in sender account.");
            }

            var restClient = new RestClient("https://api.frankfurter.app");
            var restRequest = new RestRequest("/latest", Method.Get);

            restRequest.AddParameter("from", sender.Currency);
            restRequest.AddParameter("to", receiver.Currency);

            var response = await restClient.ExecuteAsync(restRequest);
            decimal reciverRate = 1;
            if (response.StatusCode != HttpStatusCode.UnprocessableContent)
            {
                var auditError = new AuditError
                {
                    UserId = sender.UserId,
                    Action = "Transfer",
                    Description = $"Error fetching exchange rate: {response.Content}",
                    Timestamp = DateTime.UtcNow
                };
                await _requestService.SaveAuditError(auditError);

                var rate = JsonConvert.DeserializeObject<RatesServiceDto>(response.Content);
                reciverRate = rate.Rates[receiver.Currency];
            }
            

            // Update balances
            sender.Balance -= transactionServiceDto.Amount;
            receiver.Balance += transactionServiceDto.Amount * reciverRate;

            // Log transaction
            var transaction = new Transaction
            {
                SenderAccountId = sender.UserId,
                ReciverAccountId = receiver.UserId,
                Amount = transactionServiceDto.Amount,
                TransactionType = "transfer",
                Date = DateTime.UtcNow,
                Details = string.IsNullOrWhiteSpace(transactionServiceDto.Details)
                            ? "Transfer between accounts"
                            : transactionServiceDto.Details
            };

            try
            {
                _bankRepository.BankAccounts.Update(sender);
                _bankRepository.BankAccounts.Update(receiver);
                _bankRepository.Transactions.Add(transaction);

                await _bankRepository.SaveChangesAsync();

                var auditLog = new AuditLog
                {
                    UserId = sender.UserId,
                    Action = "Transfer",
                    Timestamp = DateTime.UtcNow,
                    Description = $"Transferred {transactionServiceDto.Amount} from {sender.AccountNumber} to {receiver.AccountNumber}."
                };
                await _requestService.SaveAuditLog(auditLog);

                return HttpResult.Factory.Create(HttpStatusCode.OK, new
                {
                    message = "Transfer successful",
                    senderBalance = sender.Balance,
                    receiverBalance = receiver.Balance
                });
            }
            catch (Exception ex)
            {
                var auditError = new AuditError
                {
                    UserId = sender.UserId,
                    Action = "Transfer",
                    Description = $"Error during transfer: {ex.Message}",
                    Timestamp = DateTime.UtcNow
                };
                await _requestService.SaveAuditError(auditError);

                return HttpResult.Factory.Create(HttpStatusCode.InternalServerError, null, $"Error processing transfer: {ex.Message}");
            }
        }
        public async Task<HttpResult> GetTransactionsByUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                var auditError = new AuditError
                {
                    UserId = null,
                    Action = "GetTransactionsByUser",
                    Description = "User ID cannot be null or empty.",
                    Timestamp = DateTime.UtcNow
                };
                await _requestService.SaveAuditError(auditError);

                return HttpResult.Factory.Create(HttpStatusCode.BadRequest, null, "User ID cannot be null or empty.");
            }
            var transactions = await _bankRepository.Transactions
                .Where(t => t.SenderAccount.Id.ToString() == userId || t.ReciverAccount.Id.ToString() == userId)
                .Include(t => t.SenderAccount)
                .Include(t => t.ReciverAccount)
                .ToListAsync();
            var transactionHistories = new List<TransactionHistoryServiceDto>();
            if (transactions == null || !transactions.Any())
            {
                var auditError = new AuditError
                {
                    UserId = long.Parse(userId),
                    Action = "GetTransactionsByUser",
                    Description = "No transactions found for the user.",
                    Timestamp = DateTime.UtcNow
                };
                await _requestService.SaveAuditError(auditError);

                return HttpResult.Factory.Create(HttpStatusCode.NotFound, null, "No transactions found for this user.");
            }
            else
            {

                foreach (var transaction in transactions)
                {
                    var transactionHistory = new TransactionHistoryServiceDto();
                    if (transaction.ReciverAccountId.ToString() == userId && transaction.SenderAccountId.ToString() == userId)
                    {
                        transactionHistory.Direction = "INTERNAL";
                    }
                    else if (transaction.ReciverAccountId.ToString() == userId)
                    {
                        transactionHistory.Direction = "INCOMING";
                    }
                    else if (transaction.SenderAccountId.ToString() == userId)
                    {
                        transactionHistory.Direction = "OUTGOING";
                    }

                    var reciverName = await _bankRepository.Users
                        .Where(u => u.Id == transaction.ReciverAccountId)
                        .Select(u => u.FullName)
                        .FirstOrDefaultAsync();
                    transactionHistory.ReciverName = reciverName ?? "Unknown Receiver";
                    transactionHistory.toAccountNumber = transaction.ReciverAccount.AccountNumber;
                    transactionHistory.fromAccountNumber = transaction.SenderAccount.AccountNumber;
                    transactionHistory.Amount = transaction.Amount;
                    transactionHistory.Date = transaction.Date;
                    transactionHistory.description = transaction.Details ?? "No description provided.";
                    transactionHistory.Currency = transaction.SenderAccount.Currency;



                    transactionHistories.Add(transactionHistory);
                }
            }
            var auditLog = new AuditLog
            {
                UserId = long.Parse(userId),
                Action = "GetTransactionsByUser",
                Timestamp = DateTime.UtcNow,
                Description = $"Retrieved transactions for user with ID {userId}."
            };
            await _requestService.SaveAuditLog(auditLog);

            return HttpResult.Factory.Create(HttpStatusCode.OK, transactionHistories);
        }
        public async Task<HttpResult> Deposit(DepositServiceDto depositServiceDto)
        {
            try
            {
                var reciverAccount = await _bankRepository.BankAccounts
                    .FirstOrDefaultAsync(a => a.AccountNumber == depositServiceDto.AccountNumber && a.UserId.ToString() == depositServiceDto.UserId);

                if (reciverAccount == null)
                {
                    var auditError = new AuditError
                    {
                        UserId = long.Parse(depositServiceDto.UserId),
                        Action = "Deposit",
                        Description = "Account not found.",
                        Timestamp = DateTime.UtcNow
                    };
                    await _requestService.SaveAuditError(auditError);

                    return HttpResult.Factory.Create(HttpStatusCode.NotFound, null, "Account not found.");
                }
                reciverAccount.Balance += depositServiceDto.Amount;

                _bankRepository.BankAccounts.Update(reciverAccount);

                _bankRepository.Transactions.Add(new Transaction
                {
                    ReciverAccountId = reciverAccount.Id,
                    SenderAccountId = reciverAccount.Id,
                    Amount = depositServiceDto.Amount,
                    TransactionType = "deposit",
                    Date = DateTime.UtcNow,
                    Details = $"Deposit of {depositServiceDto.Amount} to account {depositServiceDto.AccountNumber}"
                });
                await _bankRepository.SaveChangesAsync();

                var auditLog = new AuditLog
                {
                    UserId = reciverAccount.UserId,
                    Action = "Deposit",
                    Timestamp = DateTime.UtcNow,
                    Description = $"Deposited {depositServiceDto.Amount} to account {depositServiceDto.AccountNumber}."
                };

                await _requestService.SaveAuditLog(auditLog);

                return HttpResult.Factory.Create(HttpStatusCode.OK, new
                {
                    message = "Deposit successful",
                    newBalance = reciverAccount.Balance
                });
            }
            catch (Exception ex)
            {
                var auditError = new AuditError
                {
                    UserId = long.Parse(depositServiceDto.UserId),
                    Action = "Deposit",
                    Description = $"Error during deposit: {ex.Message}",
                    Timestamp = DateTime.UtcNow
                };
                await _requestService.SaveAuditError(auditError);

                return HttpResult.Factory.Create(HttpStatusCode.InternalServerError, null, "Internal server error!");
            }
        }
        public async Task<HttpResult> Withdraw(WithdrawServiceDto withdrawServiceDto)
        {
            try
            {
                var reciverAccount = await _bankRepository.BankAccounts
                    .FirstOrDefaultAsync(a => a.AccountNumber == withdrawServiceDto.AccountNumber && a.UserId.ToString() == withdrawServiceDto.UserId);

                if (reciverAccount == null)
                {
                    var auditError = new AuditError
                    {
                        UserId = long.Parse(withdrawServiceDto.UserId),
                        Action = "Withdraw",
                        Description = "Account not found.",
                        Timestamp = DateTime.UtcNow
                    };
                    await _requestService.SaveAuditError(auditError);

                    return HttpResult.Factory.Create(HttpStatusCode.NotFound, null, "Account not found.");
                }
                reciverAccount.Balance -= withdrawServiceDto.Amount;
                _bankRepository.BankAccounts.Update(reciverAccount);
                _bankRepository.Transactions.Add(new Transaction
                {
                    ReciverAccountId = reciverAccount.Id,
                    SenderAccountId = reciverAccount.Id,
                    Amount = withdrawServiceDto.Amount,
                    TransactionType = "withdrawal",
                    Date = DateTime.UtcNow,
                    Details = $"Deposit of {withdrawServiceDto.Amount} to account {withdrawServiceDto.AccountNumber}"
                });
                await _bankRepository.SaveChangesAsync();

                var auditLog = new AuditLog
                {
                    UserId = reciverAccount.UserId,
                    Action = "Withdraw",
                    Timestamp = DateTime.UtcNow,
                    Description = $"Withdrew {withdrawServiceDto.Amount} from account {withdrawServiceDto.AccountNumber}."
                };

                await _requestService.SaveAuditLog(auditLog);

                return HttpResult.Factory.Create(HttpStatusCode.OK, new
                {
                    message = "Withdraw successful",
                    newBalance = reciverAccount.Balance
                });
            }
            catch (Exception ex)
            {
                var auditError = new AuditError
                {
                    UserId = long.Parse(withdrawServiceDto.UserId),
                    Action = "Withdraw",
                    Description = $"Error during withdrawal: {ex.Message}",
                    Timestamp = DateTime.UtcNow
                };
                await _requestService.SaveAuditError(auditError);

                return HttpResult.Factory.Create(HttpStatusCode.InternalServerError, null, "Internal server error!");
            }
        }
    }
}
