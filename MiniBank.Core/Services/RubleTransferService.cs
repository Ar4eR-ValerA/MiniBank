using MiniBank.Core.Interfaces;
using MiniBank.Core.Tools;

namespace MiniBank.Core.Services
{
    public class RubleTransferService : IRubleTransferService
    {
        public int TransferRuble(int rubles, int targetCurrencyRate)
        {
            var result = rubles * targetCurrencyRate;

            if (result < 0)
            {
                throw new UserFriendlyException("Result is negative");
            }

            return result;
        }
    }
}