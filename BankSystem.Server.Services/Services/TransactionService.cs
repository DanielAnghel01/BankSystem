using BankSystem.Server.Infrastructure.DataAccess;
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
                return HttpResult.Factory.Create(HttpStatusCode.BadRequest, null, "Amount must be greater than zero.");
            }

            var sender = await _bankRepository.BankAccounts
                .FirstOrDefaultAsync(a => a.AccountNumber == transactionServiceDto.SenderAccountNumber);
            var receiver = await _bankRepository.BankAccounts
                .FirstOrDefaultAsync(a => a.AccountNumber == transactionServiceDto.ReceiverAccountNumber);

            if (sender == null || receiver == null)
            {
                return HttpResult.Factory.Create(HttpStatusCode.NotFound, null, "Sender or receiver account not found.");
            }

            if (sender.Balance < transactionServiceDto.Amount)
            {
                return HttpResult.Factory.Create(HttpStatusCode.BadRequest, null, "Insufficient balance in sender account.");
            }

            // Update balances
            sender.Balance -= transactionServiceDto.Amount;
            receiver.Balance += transactionServiceDto.Amount;

            // Log transaction
            var transaction = new Transaction
            {
                SenderAccountId = sender.Id,
                ReciverAccountId = receiver.Id,
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

                return HttpResult.Factory.Create(HttpStatusCode.OK, new
                {
                    message = "Transfer successful",
                    senderBalance = sender.Balance,
                    receiverBalance = receiver.Balance
                });
            }
            catch (Exception ex)
            {
                return HttpResult.Factory.Create(HttpStatusCode.InternalServerError, null, $"Error processing transfer: {ex.Message}");
            }
        }
    }
}
