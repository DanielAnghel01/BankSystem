using AutoMapper;
using BankSystem.Server.Dtos;
using BankSystem.Server.Services.Dtos;
using BankSystem.Server.Services.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;

namespace BankSystem.Server.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthController(AuthService authService, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            _authService = authService;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpPost("api/auth/register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // Example logic - adjust based on your setup
            if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password))
                return BadRequest("Username and password are required.");

            var user = new IdentityUser
            {
                UserName = dto.Username,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Registration successful" });
        }

        [HttpPost]
        [Route("api/auth/login")]
        public async Task<IActionResult> Login([FromBody]LoginDto loginDto)
        {
            var result = await _authService.Login(_mapper.Map<LoginServiceDto>(loginDto));
            if (result.StatusCode == (int)HttpStatusCode.OK)
            {
                return Ok(result);
            }
            else
            {
                return StatusCode(result.StatusCode, result.ErrorMessage);
            }
        }
    }
}
