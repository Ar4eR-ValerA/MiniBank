namespace MiniBank.Data.Models;

public class CurrencyModel
{
    public CurrencyModel(string charCode, double nominal, double value)
    {
        CharCode = charCode;
        Nominal = nominal;
        Value = value;
    }

    public string CharCode { get; set; }
    public double Nominal { get; set; }
    public double Value { get; set; }
}