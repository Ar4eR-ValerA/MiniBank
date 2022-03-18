namespace MiniBank.Data.Repositories.DbModels;

public class UserDbModel
{
    public Guid Id { get; set; }
    public string Login { get; set; }
    public string Email { get; set; }
}