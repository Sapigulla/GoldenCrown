using GoldenCrown.Services;
using GoldenCrown.Dtos.Finance;
using Microsoft.AspNetCore.Mvc;

namespace GoldenCrown.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinanceController : Controller
    {
        private readonly IFinanceService _financeService;

        public FinanceController(IFinanceService financeService)
        {
            _financeService = financeService;
        }

        [HttpGet("balance")] // GET api/finance/balance]
        public async Task<IActionResult> GetBalanceAsync([FromHeader] string token)
        {
            var balanceResult = await _financeService.GetBalanceAsync(token);

            if (balanceResult.IsSuccess)
            {
                return Ok(new BalanceResponse
                {
                    Balance = balanceResult.Value
                });
            }
            return BadRequest(new { Message = balanceResult.ErrorMessage });
        }

        [HttpPost("deposit")] // POST api/finance/deposit
        public async Task<IActionResult> DepositAsync([FromBody] DepositRequest request)
        {
            var depositResult = await _financeService.DepositAsync(request.Token, request.Amount);

            if (depositResult.IsSuccess)
            {
                return Ok();
            }
            return BadRequest(new { Message = depositResult.ErrorMessage });
        }

        [HttpPost("transfer")] // POST api/finance/transfer
        public async Task<IActionResult> TransferAsync([FromBody] TransferRequest request)
        {
            var transferResult = await _financeService.TransferAsync(request.Token, request.ReceiverLogin, request.Amount);

            if (transferResult.IsSuccess)
            {
                return Ok();
            }
            return BadRequest(new { Message = transferResult.ErrorMessage });
        }
    }
}
