using MiniBank.Core.Domain.Currencies.Providers;
using MiniBank.Core.Tools;

namespace MiniBank.Core.Domain.Currencies.Services
{
    public class CurrencyRateConversionService : ICurrencyRateConversionService
    {
        private readonly ICurrencyRateProvider _currencyRateProvider;
        
        public CurrencyRateConversionService(ICurrencyRateProvider currencyRateProvider)
        {
            _currencyRateProvider = currencyRateProvider;
        }
        
        public double ConvertCurrencyRate(double amount, Currency fromCurrencyCode, Currency toCurrencyCode)
        {
            if (amount < 0)
            {
                throw new UserFriendlyException("Amount is negative");
            }
            
            var result = amount * _currencyRateProvider.GetCurrencyRate(fromCurrencyCode, toCurrencyCode);

            return result;
        }
    }
}