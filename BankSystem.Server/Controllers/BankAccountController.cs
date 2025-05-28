using AutoMapper;
using BankSystem.Server.Domain.Entities;
using BankSystem.Server.Dtos;
using BankSystem.Server.Infrastructure.DataAccess;
using BankSystem.Server.Services.Dtos;
using BankSystem.Server.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace BankSystem.Server.Controllers
{
    [ApiController]
    [Route("api/bank-account")]
    public class BankAccountController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly BankAccountService _bankAccountService;
        public BankAccountController(BankAccountService bankAccountService, IMapper mapper)
        {
            _mapper = mapper;
            _bankAccountService = bankAccountService;
        }
        
        [HttpPost("create")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateBankAccountDto accountDto)
        {
            var result = await _bankAccountService.CreateAccountAsync(_mapper.Map<CreateBankAccountServiceDto>(accountDto));

            if (result.StatusCode >= 400)
                return StatusCode(result.StatusCode, new { error = result.ErrorMessage });

            return StatusCode(result.StatusCode, result.Content);
        }

        [HttpGet("by-user")]
        public async Task<IActionResult> GetAccountByUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _bankAccountService.GetAccountByUser(userId);
            return StatusCode(result.StatusCode, result.Content);
        }
    }
}

