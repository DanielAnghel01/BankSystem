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
        public async Task<HttpResult> GetAllUsers()
        {
            var users = await _bankDbContext.Users.Where(e => e.Role == "user" || e.Role == "pb").ToListAsync();
            if (users == null || !users.Any())
            {
                return HttpResult.Factory.Create(HttpStatusCode.NotFound, null, "No users found");
            }
            return HttpResult.Factory.Create(HttpStatusCode.OK, users);
        }
        public async Task<HttpResult> DeactivateUser(int userId)
        {
            var user = await _bankDbContext.Users.FirstOrDefaultAsync(e => e.Id == userId);
            if (user == null)
            {
                return HttpResult.Factory.Create(HttpStatusCode.NotFound, null, "No user found");
            }

            user.IsActive = false;

            await _requestService.UpdateUser(user);

            return HttpResult.Factory.Create(HttpStatusCode.OK, user);
        }
        public async Task<HttpResult> ActivateUser(int userId)
        {
            var user = await _bankDbContext.Users.FirstOrDefaultAsync(e => e.Id == userId);
            if (user == null)
            {
                return HttpResult.Factory.Create(HttpStatusCode.NotFound, null, "No user found");
            }

            user.IsActive = true;

            await _requestService.UpdateUser(user);

            return HttpResult.Factory.Create(HttpStatusCode.OK, user);
        }
        public async Task<HttpResult> TwoFactor(int userId)
        {
            var user = await _bankDbContext.Users.FirstOrDefaultAsync(e => e.Id == userId);
            if (user == null)
            {
                return HttpResult.Factory.Create(HttpStatusCode.NotFound, null, "No user found");
            }

            if (user.TwoFAEnabled == false)
            {
                user.TwoFAEnabled = true;
            }
            else
            {
                user.TwoFAEnabled = false;
            }

            await _requestService.UpdateUser(user);

            return HttpResult.Factory.Create(HttpStatusCode.OK, user);
        }
    }
}
