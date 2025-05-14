using AutoMapper;
using BankSystem.Server.Dtos;
using BankSystem.Server.Services.Dtos;
using BankSystem.Server.Services.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Cors;


namespace BankSystem.Server.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IMapper _mapper;

        public AuthController(AuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(_mapper.Map<RegisterServiceDto>(registerDto));

            if (result.StatusCode >= 400)
                return StatusCode(result.StatusCode, new { error = result.ErrorMessage });

            return StatusCode(result.StatusCode, result.Content);
        }


        [HttpPost("login")]
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
