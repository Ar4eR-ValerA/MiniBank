using MiniBank.Core.Entities;

namespace MiniBank.Core.Repositories;

public interface IUserRepository
{
    User GetUserById(Guid id);
    IEnumerable<User> GetAllUsers();
    bool Contains(Guid id);
    Guid CreateUser(User user);
    void UpdateUser(User user);
    void DeleteUser(Guid id);
}