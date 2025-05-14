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

        public UserService(BankDbContext bankDbContext)
        {
            _bankDbContext = bankDbContext;
        }

        public async Task<HttpResult> GetUserProfile(string userId)
        {
            var user = await _bankDbContext.Users.FirstOrDefaultAsync(e => e.Id.ToString() == userId);
            if (user == null)
            {
                return HttpResult.Factory.Create(HttpStatusCode.BadRequest, null, "User not found");
            }

            var bankAccounts = await _bankDbContext.BankAccounts.Where(e => e.UserId.ToString() == userId).ToListAsync();

            var userProfile = new UserProfileServiceDto
            {
                User = user,
                BankAccounts = bankAccounts
            };
            return HttpResult.Factory.Create(HttpStatusCode.OK, userProfile);
        }
    }
}
