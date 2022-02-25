using Microsoft.AspNetCore.Mvc;
using MiniBank.Core.Interfaces;
using MiniBank.Data.Interfaces;

namespace MiniBank.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyTransferController : ControllerBase
    {
        private IRubleTransferService _rubleTransferService;
        private ICurrencyRateProvider _currencyRateProvider;

        public CurrencyTransferController(
            IRubleTransferService rubleTransferService,
            ICurrencyRateProvider currencyRateProvider)
        {
            _rubleTransferService = rubleTransferService;
            _currencyRateProvider = currencyRateProvider;
        }

        [HttpGet(Name = "TransferCurrency")]
        public int TransferCurrency([FromQuery] int rubles, string targetCurrencyCode)
        {
            return _rubleTransferService.TransferRuble(
                rubles,
                _currencyRateProvider.GetCurrencyRate(targetCurrencyCode));
        }
    }
}