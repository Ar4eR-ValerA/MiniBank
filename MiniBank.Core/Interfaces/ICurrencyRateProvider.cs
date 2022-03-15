namespace MiniBank.Core.Interfaces
{
    public interface ICurrencyRateProvider
    {
        double GetCurrencyRate(string fromCurrencyCode, string toCurrencyCode);
    }
}