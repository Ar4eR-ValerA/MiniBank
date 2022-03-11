namespace MiniBank.Core.Interfaces
{
    public interface ICurrencyRateProvider
    {
        int GetCurrencyRate(string currencyCode);
    }
}