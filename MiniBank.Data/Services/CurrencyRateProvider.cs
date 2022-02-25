using MiniBank.Data.Interfaces;

namespace MiniBank.Data.Services
{
    public class CurrencyRateProvider : ICurrencyRateProvider
    {
        public int GetCurrencyRate(string currencyCode)
        {
            return 120;
        }
    }
}