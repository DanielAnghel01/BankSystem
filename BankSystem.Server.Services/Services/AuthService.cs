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
                var auditError = new AuditError
                {
                    UserId = null, // No user ID at registration
                    Action = "Register",
                    Description = "Invalid registration request: all fields are required.",
                    Timestamp = DateTime.UtcNow
                };
                await _requestService.SaveAuditError(auditError);

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
                var auditError = new AuditError
                {
                    UserId = null, // No user ID at registration
                    Action = "Register",
                    Description = $"Error saving user: {ex.Message}",
                    Timestamp = DateTime.UtcNow
                };
                await _requestService.SaveAuditError(auditError);

                return HttpResult.Factory.Create(HttpStatusCode.InternalServerError, null, $"Error saving user: {ex.Message}");
            }
            var auditLog = new AuditLog
            {
                UserId = user.Id,
                Action = "Register",
                Timestamp = DateTime.UtcNow,
                Description = $"User {user.Username} registered successfully"
            };
            await _requestService.SaveAuditLog(auditLog);

            return HttpResult.Factory.Create(HttpStatusCode.Created, new { message = "Registration successful" });
        }

        public async Task<HttpResult> Login(LoginServiceDto loginServiceDto)
        {
            try
            {
                var user = await _bankRepository.Users.FirstOrDefaultAsync(u => u.Username == loginServiceDto.UserName && u.Password == loginServiceDto.Password);

                if (user == null)
                {
                    var auditError = new AuditError
                    {
                        UserId = null, // No user ID at login
                        Action = "Login",
                        Description = "Invalid username or password.",
                        Timestamp = DateTime.UtcNow
                    };
                    await _requestService.SaveAuditError(auditError);

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
                var auditLog = new AuditLog
                {
                    UserId = user.Id,
                    Action = "Login",
                    Timestamp = DateTime.UtcNow,
                    Description = $"User {user.Username} logged in successfully"
                };
                await _requestService.SaveAuditLog(auditLog);

                return HttpResult.Factory.Create(HttpStatusCode.OK, loginResponse);
            }
            catch (Exception ex)
            {
                var auditError = new AuditError
                {
                    UserId = null, // No user ID at login
                    Action = "Login",
                    Description = $"Error during login: {ex.Message}",
                    Timestamp = DateTime.UtcNow
                };
                await _requestService.SaveAuditError(auditError);

                return HttpResult.Factory.Create(HttpStatusCode.InternalServerError, null, "Internal server error");
            }
        }
    }
}
