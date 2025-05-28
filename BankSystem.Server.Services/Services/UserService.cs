using BankSystem.Server.Domain.Entities;
using BankSystem.Server.Infrastructure.DataAccess;
using BankSystem.Server.Services.Dtos;
using BankSystem.Server.Services.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Server.Services.Services
{
    public class UserService
    {
        private readonly BankDbContext _bankDbContext;
        private readonly RequestService _requestService;

        public UserService(BankDbContext bankDbContext, RequestService requestService)
        {
            _bankDbContext = bankDbContext;
            _requestService = requestService;
        }

        public async Task<HttpResult> GetUserProfile(string userId)
        {
            var user = await _bankDbContext.Users.FirstOrDefaultAsync(e => e.Id.ToString() == userId);
            if (user == null)
            {
                var auditError = new AuditError
                {
                    UserId = (long)Convert.ToDouble(userId),
                    Action = "GetUserProfile",
                    Description = "User not found",
                    Timestamp = DateTime.UtcNow
                };
                await _requestService.SaveAuditError(auditError);

                return HttpResult.Factory.Create(HttpStatusCode.BadRequest, null, "User not found");
            }

            var bankAccounts = await _bankDbContext.BankAccounts.Where(e => e.UserId.ToString() == userId).ToListAsync();

            var userProfile = new UserProfileServiceDto
            {
                User = user,
                BankAccounts = bankAccounts
            };

            var auditLog = new AuditLog
            {
                UserId = user.Id,
                Action = "GetUserProfile",
                Timestamp = DateTime.UtcNow,
                Description = $"Retrieved profile for user {userId}"
            };

            await _requestService.SaveAuditLog(auditLog);

            return HttpResult.Factory.Create(HttpStatusCode.OK, userProfile);
        }
    }
}
