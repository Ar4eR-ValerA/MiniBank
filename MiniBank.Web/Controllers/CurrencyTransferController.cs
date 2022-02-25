using Microsoft.AspNetCore.Mvc;
using MiniBank.Core.Interfaces;

namespace MiniBank.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyTransferController : ControllerBase
    {
        private readonly IRubleTransferService _rubleTransferService;

        public CurrencyTransferController(IRubleTransferService rubleTransferService)
        {
            _rubleTransferService = rubleTransferService;
        }

        [HttpGet(Name = "TransferCurrency")]
        public int TransferCurrency([FromQuery] int rubles, string targetCurrencyCode)
        {
            return _rubleTransferService.TransferRuble(rubles, targetCurrencyCode);
        }
    }
}