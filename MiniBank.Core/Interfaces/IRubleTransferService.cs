namespace MiniBank.Core.Interfaces
{
    public interface IRubleTransferService
    {
        int TransferRuble(int rubles, string targetCurrencyCode, ICurrencyRateProvider currencyRateProvider);
    }
}