using System;
using MiniBank.Core.Interfaces;

namespace MiniBank.Core.Services
{
    public class RubleTransferService : IRubleTransferService
    {
        public int TransferRuble(int rubles, int targetCurrencyRate)
        {
            var result = rubles * targetCurrencyRate;

            if (result < 0)
            {
                // TODO: Сделать раздельные юзер ошибки и внутренние
                throw new Exception("Result is negative");
            }

            return result;
        }
    }
}