using MiniBank.Core.Entities;

namespace MiniBank.Core.Services.Interfaces;

public interface IUserService
{
    User GetById(Guid id);
    IEnumerable<User> GetAll();
    Guid Create(User user);
    void Update(User user);
    void Delete(Guid id);
}