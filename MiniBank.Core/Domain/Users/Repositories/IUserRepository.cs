namespace MiniBank.Core.Domain.Users.Repositories;

public interface IUserRepository
{
    Task<bool> IsExist(Guid id);
    Task<bool> IsLoginExists(string login);
    Task<User> GetById(Guid id);
    Task<IReadOnlyList<User>> GetAll();
    Task Create(User user);
    Task Update(User user);
    Task Delete(Guid id);
}