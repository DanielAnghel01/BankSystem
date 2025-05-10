using AutoMapper;
using BankSystem.Server.Infrastructure.DataAccess;
using BankSystem.Server.Services.Dtos;
using BankSystem.Server.Services.Utils;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Microsoft.AspNetCore.Identity;
using BankSystem.Server.Domain.Entities;

namespace BankSystem.Server.Services.Services
{   
    public class AuthService
    {
        private readonly BankDbContext _bankRepository;
        private readonly IMapper _mapper;
        private readonly RequestService _requestService;
        public AuthService(BankDbContext bankRepository, IMapper mapper, RequestService requestService)
        {
            _bankRepository = bankRepository;
            _mapper = mapper;
            _requestService = requestService;
        }

        public async Task<(bool isSuccessful, object result)> RegisterAsync(RegisterServiceDto registerServiceDto)
        {
            if (string.IsNullOrWhiteSpace(registerServiceDto.Username) ||
                string.IsNullOrWhiteSpace(registerServiceDto.Password))
            {
                return (false, "All fields are required.");
            }

            var password = registerServiceDto.Password;

            // Create user
            var user = new User
            {
                Username = registerServiceDto.Username,
                Email = registerServiceDto.Email,
                Password = registerServiceDto.Password,
                FullName = registerServiceDto.FullName,
                Role = "user",
                DateOfBirth = registerServiceDto.DateOfBirth,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _requestService.SaveUser(user);
            }
            catch (Exception ex)
            {
                return (false, $"Error saving user: {ex.Message}");
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
