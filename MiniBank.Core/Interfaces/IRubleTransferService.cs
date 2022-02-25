namespace MiniBank.Core.Interfaces
{
    public interface IRubleTransferService
    {
        int TransferRuble(int rubles, int targetCurrencyRate);
    }
}