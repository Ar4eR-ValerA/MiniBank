using MiniBank.Core.Interfaces;
using MiniBank.Core.Tools;

namespace MiniBank.Core.Services
{
    public class RubleTransferService : IRubleTransferService
    {
        private readonly ICurrencyRateProvider _currencyRateProvider;
        
        public RubleTransferService(ICurrencyRateProvider currencyRateProvider)
        {
            _currencyRateProvider = currencyRateProvider;
        }
        
        public int TransferRuble(int rubles, string targetCurrencyCode)
        {
            var result = rubles * _currencyRateProvider.GetCurrencyRate(targetCurrencyCode);

            if (result < 0)
            {
                throw new UserFriendlyException("Result is negative");
            }

            return result;
        }
    }
}