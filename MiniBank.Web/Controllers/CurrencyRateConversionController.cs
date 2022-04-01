using Microsoft.AspNetCore.Mvc;
using MiniBank.Core.Domain.Currencies;
using MiniBank.Core.Domain.Currencies.Services;

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
        public async Task<double> ConvertRubleRate(double amount, Currency fromCurrencyCode, Currency toCurrencyCode)
        {
            return await _currencyRateConversionService.ConvertCurrencyRate(amount, fromCurrencyCode, toCurrencyCode);
        }
    }
}