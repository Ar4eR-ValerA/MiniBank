namespace MiniBank.Web.Dtos;

public class AccountInfoDto
{
    public Guid UserId { get; set; }
    public double Balance { get; set; }
    public string Currency { get; set; }
    public DateTime DateClosed { get; set; }
}