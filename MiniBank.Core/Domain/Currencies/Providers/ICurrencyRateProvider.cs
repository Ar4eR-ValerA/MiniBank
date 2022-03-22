namespace MiniBank.Core.Domain.Currencies.Providers
{
    public interface ICurrencyRateProvider
    {
        double GetCurrencyRate(Currency fromCurrencyCode, Currency toCurrencyCode);
    }
}