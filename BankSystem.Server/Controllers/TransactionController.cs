using Microsoft.AspNetCore.Mvc;
using BankSystem.Server.Services.Dtos;
using BankSystem.Server.Services.Services;
using AutoMapper;
using BankSystem.Server.Dtos;

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
            var serviceDto = _mapper.Map<TransactionServiceDto>(dto);
            var result = await _transactionService.TransferAsync(serviceDto);

            if (result.StatusCode >= 400)
                return StatusCode(result.StatusCode, new { error = result.ErrorMessage });

            return StatusCode(result.StatusCode, result.Content ?? new { error = result.ErrorMessage });
        }
    }
}
