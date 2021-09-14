using System;
using System.Threading.Tasks;
using D3.AccountLedger.Api.Grains;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace D3.AccountLedger.Api.Controllers
{
    [ApiController]
    [Route("[controller]/{accountNumber}")]
    public class AccountsController : ControllerBase
    {
        private readonly IGrainFactory _grainFactory;

        public AccountsController(IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }

        [HttpPost("debits/{transactionId}/{amount:decimal}")]
        public async Task<IActionResult> PostDebits(string accountNumber, string transactionId, decimal amount)
        {
            var grain = _grainFactory.GetGrain<IAccountLedgerGrain>(accountNumber);

            var transactionTicket =  await grain.AddDebit(new DebitTransaction
                {Amount = amount, TransactionDate = DateTimeOffset.UtcNow, TransactionId = transactionId});

            return Ok(transactionTicket);
        }

        [HttpPost("credits/{transactionId}/{amount:decimal}")]
        public async Task<IActionResult> PostCredits(string accountNumber, string transactionId, decimal amount)
        {
            var grain = _grainFactory.GetGrain<IAccountLedgerGrain>(accountNumber);

            var transactionTicket = await grain.AddCredit(new CreditTransaction
                { Amount = amount, TransactionDate = DateTimeOffset.UtcNow, TransactionId = transactionId });

            return Ok(transactionTicket);
        }

        [HttpGet]
        public async Task<IActionResult> GetAccountBalance(string accountNumber)
        {
            var grain = _grainFactory.GetGrain<IAccountLedgerGrain>(accountNumber);

            return Ok(await grain.GetCurrentBalance());
        }
    }
}
