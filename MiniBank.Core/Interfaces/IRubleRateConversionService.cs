namespace MiniBank.Core.Interfaces
{
    public interface IRubleRateConversionService
    {
        int ConvertRubleRate(int rubles, string targetCurrencyCode);
    }
}