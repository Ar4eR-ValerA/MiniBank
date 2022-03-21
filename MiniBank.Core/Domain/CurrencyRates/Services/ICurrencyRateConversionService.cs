namespace MiniBank.Core.Domain.CurrencyRates.Services
{
    public interface ICurrencyRateConversionService
    {
        double ConvertCurrencyRate(double amount, string fromCurrencyCode, string toCurrencyCode);
    }
}