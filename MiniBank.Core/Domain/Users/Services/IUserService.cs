namespace MiniBank.Core.Domain.Users.Services;

public interface IUserService
{
    User GetById(Guid id);
    IEnumerable<User> GetAll();
    Guid Create(User user);
    void Update(User user);
    void Delete(Guid id);
}