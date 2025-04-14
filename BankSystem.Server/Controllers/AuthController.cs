using AutoMapper;
using BankSystem.Server.Dtos;
using BankSystem.Server.Services.Dtos;
using BankSystem.Server.Services.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Cors;

namespace BankSystem.Server.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IMapper _mapper;

        public AuthController(AuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }

        [EnableCors("AllowAll")]
        [HttpPost]
        [Route("api/auth/login")]
        public async Task<IActionResult> Login([FromBody]LoginDto loginDto)
        {
            var result = await _authService.Login(_mapper.Map<LoginServiceDto>(loginDto));
            if (result.StatusCode == (int)HttpStatusCode.OK)
            {
                return Ok(result.Content);
            }
            else
            {
                return StatusCode(result.StatusCode, result.ErrorMessage);
            }
        }
    }
}
