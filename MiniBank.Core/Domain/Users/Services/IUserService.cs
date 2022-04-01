namespace MiniBank.Core.Domain.Users.Services;

public interface IUserService
{
    Task<User> GetById(Guid id);
    Task<IEnumerable<User>> GetAll();
    Task<Guid> Create(User user);
    Task Update(User user);
    Task Delete(Guid id);
}