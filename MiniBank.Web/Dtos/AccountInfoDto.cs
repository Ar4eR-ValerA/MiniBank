using MiniBank.Core.Tools;

namespace MiniBank.Web.Dtos;

public class AccountInfoDto
{
    private string _currency = string.Empty;

    public Guid UserId { get; set; }
    public double Balance { get; set; }

    public string Currency
    {
        get => _currency;
        set => _currency = value ?? throw new ValidationException("Currency is null");
    }

    public DateTime DateClosed { get; set; }
}