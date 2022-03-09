using MiniBank.Core.Interfaces;
using MiniBank.Core.Tools;

namespace MiniBank.Core.Services
{
    public class RubleRateConversionService : IRubleRateConversionService
    {
        private readonly ICurrencyRateProvider _currencyRateProvider;
        
        public RubleRateConversionService(ICurrencyRateProvider currencyRateProvider)
        {
            _currencyRateProvider = currencyRateProvider;
        }
        
        public int ConvertRubleRate(int rubles, string targetCurrencyCode)
        {
            if (rubles < 0)
            {
                throw new UserFriendlyException("Amount is negative");
            }
            
            var result = rubles * _currencyRateProvider.GetCurrencyRate(targetCurrencyCode);

            return result;
        }
    }
}