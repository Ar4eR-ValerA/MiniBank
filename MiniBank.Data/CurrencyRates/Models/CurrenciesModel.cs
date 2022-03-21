namespace MiniBank.Data.CurrencyRates.Models;

public class CurrenciesModel
{
    public CurrenciesModel(Dictionary<string, CurrencyModel> valute)
    {
        Valute = valute;
    }

    public Dictionary<string, CurrencyModel> Valute { get; set; }
}