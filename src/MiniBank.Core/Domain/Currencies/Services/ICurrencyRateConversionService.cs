namespace MiniBank.Core.Domain.Currencies.Services
{
    public interface ICurrencyRateConversionService
    {
        Task<double> ConvertCurrencyRate(double amount, Currency fromCurrencyCode, Currency toCurrencyCode);
    }
}