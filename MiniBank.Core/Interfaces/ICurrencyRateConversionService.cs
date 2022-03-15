namespace MiniBank.Core.Interfaces
{
    public interface ICurrencyRateConversionService
    {
        int ConvertCurrencyRate(int amount, string fromCurrencyCode, string toCurrencyCode);
    }
}