using AutoMapper;
using BankSystem.Server.Infrastructure.DataAccess;
using BankSystem.Server.Services.Dtos;
using BankSystem.Server.Services.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BankSystem.Server.Services.Services
{   
    public class AuthService
    {
        private readonly BankDbContext _bankRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;
        public AuthService(BankDbContext bankRepository, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            _bankRepository = bankRepository;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<(bool isSuccessful, object result)> RegisterAsync(RegisterServiceDto registerServiceDto)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(registerServiceDto.Username) ||
                string.IsNullOrWhiteSpace(registerServiceDto.Password))
            {
                return (false, "All fields are required.");
            }

            // Create user
            var user = new IdentityUser
            {
                UserName = registerServiceDto.Username,
                Email = registerServiceDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerServiceDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return (false, errors);  // Return only error descriptions
            }

            return (true, new { message = "Registration successful" });
        }


        public async Task<HttpResult> Login(LoginServiceDto loginServiceDto)
        {
            try
            {
                var IsValidUser = await _bankRepository.Users.AnyAsync(u => u.Username == loginServiceDto.UserName && u.Password == loginServiceDto.Password);

                if(IsValidUser)
                {
                    return HttpResult.Factory.Create(HttpStatusCode.OK, "Login succesfully");
                }
                else
                {
                    return HttpResult.Factory.Create(HttpStatusCode.BadRequest, null, "Username or Password is incorect");
                }    
            }
            catch(Exception ex)
            {
                //_logger.Error(ex);
                return HttpResult.Factory.Create(HttpStatusCode.InternalServerError, null, "Internal server error");
            }
        }
    }
}
