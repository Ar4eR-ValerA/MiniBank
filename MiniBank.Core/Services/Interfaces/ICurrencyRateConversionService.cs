namespace MiniBank.Core.Services.Interfaces
{
    public interface ICurrencyRateConversionService
    {
        double ConvertCurrencyRate(double amount, string fromCurrencyCode, string toCurrencyCode);
    }
}