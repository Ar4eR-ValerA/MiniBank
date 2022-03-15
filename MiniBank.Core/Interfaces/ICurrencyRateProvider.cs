namespace MiniBank.Core.Interfaces
{
    public interface ICurrencyRateProvider
    {
        int GetCurrencyRate(string fromCurrencyCode, string toCurrencyCode);
    }
}