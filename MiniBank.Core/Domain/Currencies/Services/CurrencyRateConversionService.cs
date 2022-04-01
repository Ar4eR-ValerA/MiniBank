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
        
        public async Task<double> ConvertCurrencyRate(double amount, Currency fromCurrencyCode, Currency toCurrencyCode)
        {
            if (amount < 0)
            {
                throw new UserFriendlyException("Amount is negative");
            }
            
            double currencyRate = await _currencyRateProvider.GetCurrencyRate(fromCurrencyCode, toCurrencyCode);
            double result = amount * currencyRate;

            return result;
        }
    }
}