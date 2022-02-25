namespace MiniBank.Data.Interfaces
{
    public interface ICurrencyRateProvider
    {
        int GetCurrencyRate(string currencyCode);
    }
}