using MiniBank.Core.Entities;

namespace MiniBank.Core.Services.Interfaces;

public interface IUserService
{
    User GetById(Guid id);
    IEnumerable<User> GetAll();
    Guid CreateUser(User user);
    void UpdateUser(User user);
    void DeleteUser(Guid id);
}