using BankSystem.Server.Domain.Entities;
using BankSystem.Server.Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Server.Services.Services
{
    public class RequestService
    {
        private readonly BankDbContext _bankRepository;
        public RequestService(BankDbContext bankRepository)
        {
            _bankRepository = bankRepository;
        }

        public async Task SaveUser(User user)
        {
            try
            {
                _bankRepository.Users.Add(user);

                await _bankRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdateUser(User user)
        {
            try
            {
                _bankRepository.Users.Update(user);

                await _bankRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SaveBankAccount(BankAccount account)
        {
            try
            {
                _bankRepository.BankAccounts.Add(account);
                await _bankRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task SaveAuditLog(AuditLog log)
        {
            try
            {
                _bankRepository.AuditLogs.Add(log);
                await _bankRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task SaveAuditError(AuditError error)
        {
            try
            {
                _bankRepository.AuditErrors.Add(error);
                await _bankRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
