namespace MiniBank.Core.Interfaces
{
    public interface ICurrencyRateConversionService
    {
        double ConvertCurrencyRate(double amount, string fromCurrencyCode, string toCurrencyCode);
    }
}