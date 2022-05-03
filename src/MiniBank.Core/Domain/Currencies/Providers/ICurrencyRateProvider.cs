namespace MiniBank.Core.Domain.Currencies.Providers
{
    public interface ICurrencyRateProvider
    {
        Task<double> GetCurrencyRate(Currency fromCurrencyCode, Currency toCurrencyCode);
    }
}