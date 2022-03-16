namespace MiniBank.Core.Services.Interfaces
{
    public interface ICurrencyRateProvider
    {
        double GetCurrencyRate(string fromCurrencyCode, string toCurrencyCode);
    }
}