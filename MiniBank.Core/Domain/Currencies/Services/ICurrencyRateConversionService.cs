namespace MiniBank.Core.Domain.Currencies.Services
{
    public interface ICurrencyRateConversionService
    {
        double ConvertCurrencyRate(double amount, Currency fromCurrencyCode, Currency toCurrencyCode);
    }
}