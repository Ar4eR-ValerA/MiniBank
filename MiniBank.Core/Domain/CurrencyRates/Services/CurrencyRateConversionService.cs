using MiniBank.Core.Domain.CurrencyRates.Providers;
using MiniBank.Core.Tools;

namespace MiniBank.Core.Domain.CurrencyRates.Services
{
    public class CurrencyRateConversionService : ICurrencyRateConversionService
    {
        private readonly ICurrencyRateProvider _currencyRateProvider;
        
        public CurrencyRateConversionService(ICurrencyRateProvider currencyRateProvider)
        {
            _currencyRateProvider = currencyRateProvider;
        }
        
        public double ConvertCurrencyRate(double amount, string fromCurrencyCode, string toCurrencyCode)
        {
            if (amount < 0)
            {
                throw new ValidationException("Amount is negative");
            }
            
            var result = amount * _currencyRateProvider.GetCurrencyRate(fromCurrencyCode, toCurrencyCode);

            return result;
        }
    }
}