using Microsoft.AspNetCore.Mvc;
using MiniBank.Core.Interfaces;

namespace MiniBank.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyRateConversionController : ControllerBase
    {
        private readonly IRubleRateConversionService _rubleRateConversionService;

        public CurrencyRateConversionController(IRubleRateConversionService rubleRateConversionService)
        {
            _rubleRateConversionService = rubleRateConversionService;
        }

        [HttpGet(Name = "ConvertRubleRate")]
        public int ConvertRubleRate([FromQuery] int rubles, string targetCurrencyCode)
        {
            return _rubleRateConversionService.ConvertRubleRate(rubles, targetCurrencyCode);
        }
    }
}