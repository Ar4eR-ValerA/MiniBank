namespace MiniBank.Core.Domain.CurrencyRates.Providers
{
    public interface ICurrencyRateProvider
    {
        double GetCurrencyRate(string fromCurrencyCode, string toCurrencyCode);
    }
}