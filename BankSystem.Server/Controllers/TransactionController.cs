using Microsoft.AspNetCore.Mvc;
using BankSystem.Server.Services.Dtos;
using BankSystem.Server.Services.Services;
using AutoMapper;
using BankSystem.Server.Dtos;
using System.Security.Claims;

namespace BankSystem.Server.Controllers
{
    [ApiController]
    [Route("api/transaction")]
    public class TransactionController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly TransactionService _transactionService;

        public TransactionController(TransactionService transactionService, IMapper mapper)
        {
            _transactionService = transactionService;
            _mapper = mapper;
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransactionDto dto)
        {
            var result = await _transactionService.TransferAsync(_mapper.Map<TransactionServiceDto>(dto));

            if (result.StatusCode >= 400)
                return StatusCode(result.StatusCode, new { error = result.ErrorMessage });

            return StatusCode(result.StatusCode, result.Content ?? new { error = result.ErrorMessage });
        }

        [HttpGet("by-user")]
        public async Task<IActionResult> GetTransactionsByUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _transactionService.GetTransactionsByUser(userId);
            return StatusCode(result.StatusCode, result.Content);
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit(DepositDto depositDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var depositServiceDto = new DepositServiceDto
            {
                AccountNumber = depositDto.AccountNumber,
                Amount = depositDto.Amount,
                UserId = userId
            };
            var result = await _transactionService.Deposit(depositServiceDto);
            if (result.StatusCode >= 400)
                return StatusCode(result.StatusCode, new { error = result.ErrorMessage });
            return StatusCode(result.StatusCode, result.Content ?? new { error = result.ErrorMessage });
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw(WithdrawDto withdrawDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var withdrawServiceDto = new WithdrawServiceDto
            {
                AccountNumber = withdrawDto.AccountNumber,
                Amount = withdrawDto.Amount,
                UserId = userId
            };
            var result = await _transactionService.Withdraw(withdrawServiceDto);
            if (result.StatusCode >= 400)
                return StatusCode(result.StatusCode, new { error = result.ErrorMessage });
            return StatusCode(result.StatusCode, result.Content ?? new { error = result.ErrorMessage });
        }
    }
}
