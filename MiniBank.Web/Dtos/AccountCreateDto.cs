namespace MiniBank.Web.Dtos;

public class AccountCreateDto
{
    public Guid UserId { get; set; }
    public double Balance { get; set; }
    public string Currency { get; set; }
}