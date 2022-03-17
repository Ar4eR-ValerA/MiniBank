using MiniBank.Core.Entities;
using MiniBank.Core.Entities.Builders;

namespace MiniBank.Core.Services.Interfaces;

public interface IUserService
{
    User GetById(Guid id);
    IEnumerable<User> GetAll();
    Guid CreateUser(UserBuilder userBuilder);
    void UpdateUser(Guid id, UserBuilder userBuilder);
    void DeleteUser(Guid id);
}