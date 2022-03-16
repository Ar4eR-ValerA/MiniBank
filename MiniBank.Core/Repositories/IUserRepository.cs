using MiniBank.Core.Entities;

namespace MiniBank.Core.Repositories;

public interface IUserRepository
{
    User GetUserById(Guid id);
    IEnumerable<User> GetAllUsers();
    Guid CreateUser(User user);
    void UpdateUser(User user);
    void DeleteUser(Guid id);
}