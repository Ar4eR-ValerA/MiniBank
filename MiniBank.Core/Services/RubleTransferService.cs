using MiniBank.Core.Interfaces;
using MiniBank.Core.Tools;

namespace MiniBank.Core.Services
{
    public class RubleTransferService : IRubleTransferService
    {
        public int TransferRuble(int rubles, string targetCurrencyCode, ICurrencyRateProvider currencyRateProvider)
        {
            var result = rubles * currencyRateProvider.GetCurrencyRate(targetCurrencyCode);

            if (result < 0)
            {
                throw new UserFriendlyException("Result is negative");
            }

            return result;
        }
    }
}