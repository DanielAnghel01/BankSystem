using Microsoft.AspNetCore.Mvc;
using BankSystem.Server.Services.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BankSystem.Server.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IMapper _mapper;

        public UserController(UserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var result = await _userService.GetUserProfile(userId);
            return StatusCode(result.StatusCode, result.Content);
        }
        [HttpGet("get-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllUsers();
            return StatusCode(result.StatusCode, result.Content);
        }
        [HttpPost("deactivate/{userId}")]
        public async Task<IActionResult> DeactivateUser(int userId)
        {
            var result = await _userService.DeactivateUser(userId);
            return StatusCode(result.StatusCode, result.Content);
        }
        [HttpPost("activate/{userId}")]
        public async Task<IActionResult> ActivateUser(int userId)
        {
            var result = await _userService.ActivateUser(userId);
            return StatusCode(result.StatusCode, result.Content);
        }
        [HttpPost("tfa/{userId}")]
        public async Task<IActionResult> TwoFactor(int userId)
        {
            var result = await _userService.TwoFactor(userId);
            return StatusCode(result.StatusCode, result.Content);
        }
    }
}
