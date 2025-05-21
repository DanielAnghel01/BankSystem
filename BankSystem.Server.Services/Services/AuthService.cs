using AutoMapper;
using BankSystem.Server.Infrastructure.DataAccess;
using BankSystem.Server.Services.Dtos;
using BankSystem.Server.Services.Utils;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Microsoft.AspNetCore.Identity;
using BankSystem.Server.Domain.Entities;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace BankSystem.Server.Services.Services
{
    public class AuthService
    {
        private readonly BankDbContext _bankRepository;
        private readonly IMapper _mapper;
        private readonly RequestService _requestService;
        private readonly IConfiguration _config;

        public AuthService(BankDbContext bankRepository, IMapper mapper, RequestService requestService, IConfiguration config)
        {
            _bankRepository = bankRepository;
            _mapper = mapper;
            _requestService = requestService;
            _config = config;
        }

        public async Task<HttpResult> RegisterAsync(RegisterServiceDto registerServiceDto)
        {
            if (string.IsNullOrWhiteSpace(registerServiceDto.Username) ||
                string.IsNullOrWhiteSpace(registerServiceDto.Password))
            {
                return HttpResult.Factory.Create(HttpStatusCode.BadRequest, null, "All fields are required.");
            }

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
                return HttpResult.Factory.Create(HttpStatusCode.InternalServerError, null, $"Error saving user: {ex.Message}");
            }

            return HttpResult.Factory.Create(HttpStatusCode.Created, new { message = "Registration successful" });
        }



        public async Task<HttpResult> Login(LoginServiceDto loginServiceDto)
        {
            try
            {
                var user = await _bankRepository.Users.FirstOrDefaultAsync(u => u.Username == loginServiceDto.UserName && u.Password == loginServiceDto.Password);

                if (user == null)
                {
                    return HttpResult.Factory.Create(HttpStatusCode.BadRequest, null, "Username or Password is incorrect");
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]);

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(double.Parse(_config["JwtSettings:ExpireMinutes"])),
                    Issuer = _config["JwtSettings:Issuer"],
                    Audience = _config["JwtSettings:Audience"],
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwt = tokenHandler.WriteToken(token);


                var loginResponse = new LoginResponseServiceDto
                {
                    Username = user.Username,
                    Token = jwt
                };
                return HttpResult.Factory.Create(HttpStatusCode.OK, loginResponse);
            }
            catch (Exception ex)
            {
                //_logger.Error(ex);
                return HttpResult.Factory.Create(HttpStatusCode.InternalServerError, null, "Internal server error");
            }
        }
    }
}
