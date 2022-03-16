using Microsoft.AspNetCore.Mvc;
using MiniBank.Core.Services.Interfaces;

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

        [HttpGet]
        public double ConvertRubleRate([FromQuery] double amount, string fromCurrencyCode, string toCurrencyCode)
        {
            return _currencyRateConversionService.ConvertCurrencyRate(amount, fromCurrencyCode, toCurrencyCode);
        }
    }
}