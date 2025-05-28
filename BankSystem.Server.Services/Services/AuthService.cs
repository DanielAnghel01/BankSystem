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
using System.Security.Cryptography;
using System;
using SendGrid;
using SendGrid.Helpers.Mail;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Mail;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BankSystem.Server.Services.Services
{
    public class AuthService
    {
        private readonly BankDbContext _bankRepository;
        private readonly IMapper _mapper;
        private readonly RequestService _requestService;
        private readonly IConfiguration _config;
        private readonly string _sendGridApiKey;

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
                Password = HashPassword(registerServiceDto.Password, GenerateSalt()),
                FullName = registerServiceDto.FullName,
                DateOfBirth = registerServiceDto.DateOfBirth.ToUniversalTime(),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            if (registerServiceDto.Role != null)
            {
                user.Role = registerServiceDto.Role;
            }
            else
            {
                user.Role = "user";
            }

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
                var user = await _bankRepository.Users.FirstOrDefaultAsync(u => u.Username == loginServiceDto.UserName);

                if (user == null)
                {
                    return HttpResult.Factory.Create(HttpStatusCode.BadRequest, null, "Username is incorrect");
                }

                if(!user.IsActive)
                {
                    return HttpResult.Factory.Create(HttpStatusCode.Unauthorized, null, "User not active");
                }

                if(!(VerifyPassword(loginServiceDto.Password, user.Password)))
                {
                    return HttpResult.Factory.Create(HttpStatusCode.BadRequest, null, "Password is incorrect");
                }

                var twoFACode = generate2FACode();

                var msj = Send2FAEmailAsync(user.Email, twoFACode);

                user.TwoFACode = twoFACode;

                await _requestService.UpdateUser(user);

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]);

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
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
                    Token = jwt,
                    TwoFAEnabled = user.TwoFAEnabled
                };
                return HttpResult.Factory.Create(HttpStatusCode.OK, loginResponse);
            }
            catch (Exception ex)
            {
                //_logger.Error(ex);
                return HttpResult.Factory.Create(HttpStatusCode.InternalServerError, null, "Internal server error");
            }
        }
        public async Task<HttpResult> Verify2FA(TwoFactorServiceDto twoFactorServiceDto)
        {
            try
            {
                var user = await _bankRepository.Users.FirstOrDefaultAsync(u => u.Username == twoFactorServiceDto.Username);

                if(user.TwoFACode == twoFactorServiceDto.Code)
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]);

                    var claims = new[]
                    {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
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
                return HttpResult.Factory.Create(HttpStatusCode.BadRequest, null, "Invalid 2FA code. Please try again.");
            }
            catch (Exception ex)
            {
                return HttpResult.Factory.Create(HttpStatusCode.InternalServerError, null, "Internal server error");
            }
        }
        private static byte[] GenerateSalt(int size = 16)
        {
            var salt = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
        private static string HashPassword(string password, byte[] salt)
        {
            using (var sha256 = SHA256.Create())
            {
                // Combine password and salt
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] passwordWithSalt = new byte[passwordBytes.Length + salt.Length];
                Buffer.BlockCopy(passwordBytes, 0, passwordWithSalt, 0, passwordBytes.Length);
                Buffer.BlockCopy(salt, 0, passwordWithSalt, passwordBytes.Length, salt.Length);

                // Compute hash
                byte[] hashBytes = sha256.ComputeHash(passwordWithSalt);

                // Combine salt and hash for storage
                byte[] hashWithSaltBytes = new byte[salt.Length + hashBytes.Length];
                Buffer.BlockCopy(salt, 0, hashWithSaltBytes, 0, salt.Length);
                Buffer.BlockCopy(hashBytes, 0, hashWithSaltBytes, salt.Length, hashBytes.Length);

                // Convert to Base64 for storage
                return Convert.ToBase64String(hashWithSaltBytes);
            }
        }
        private static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            // Decode the stored hash
            byte[] hashWithSaltBytes = Convert.FromBase64String(storedHash);

            // Extract salt (first 16 bytes)
            byte[] salt = new byte[16];
            Buffer.BlockCopy(hashWithSaltBytes, 0, salt, 0, salt.Length);

            // Hash the entered password with the extracted salt
            string enteredHash = HashPassword(enteredPassword, salt);

            // Compare the hashes
            return storedHash == enteredHash;
        }
        private async Task<HttpResponseMessage> Send2FAEmailAsync(string toEmail, string code)
        {
            var apiToken = "mlsn.f12e6924b68a84141f6cea0b75b334a10ed115d01c553be2b828ad5bc44fd950";
            var apiUrl = "https://api.mailersend.com/v1/email";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);

            var emailData = new
            {
                from = new { email = "no-reply@test-nrw7gymr11jg2k8e.mlsender.net", name = "Rock Solid Bank" },
                to = new[] { new { email = toEmail} },
                subject = "Your 2FA Verification Code",
                text = $"Your verification code is: {code}"
            };

            var content = new StringContent(JsonSerializer.Serialize(emailData), Encoding.UTF8, "application/json");

            return await httpClient.PostAsync(apiUrl, content);
        }
        private static string generate2FACode()
        {
            var random = new Random();
            int code = random.Next(100000, 1000000);
            return code.ToString();
        }
    }
}
