using MiniBank.Core.Entities;

namespace MiniBank.Core.Repositories;

public interface IUserRepository
{
    User GetById(Guid id);
    IEnumerable<User> GetAll();
    Guid CreateUser(User user);
    void UpdateUser(User user);
    void DeleteUser(Guid id);
}