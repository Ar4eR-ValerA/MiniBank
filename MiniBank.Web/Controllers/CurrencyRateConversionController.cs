using Microsoft.AspNetCore.Mvc;
using MiniBank.Core.Interfaces;

namespace MiniBank.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyRateConversionController : ControllerBase
    {
        private readonly ICurrencyRateConversionService _currencyRateConversionService;

        public CurrencyRateConversionController(ICurrencyRateConversionService currencyRateConversionService)
        {
            _currencyRateConversionService = currencyRateConversionService;
        }

        [HttpGet(Name = "ConvertCurrencyRate")]
        public int ConvertRubleRate([FromQuery] int amount, string fromCurrencyCode, string targetCurrencyCode)
        {
            return _currencyRateConversionService.ConvertCurrencyRate(amount, fromCurrencyCode, targetCurrencyCode);
        }
    }
}