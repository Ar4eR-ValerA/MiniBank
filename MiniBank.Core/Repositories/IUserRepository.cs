using MiniBank.Core.Entities;

namespace MiniBank.Core.Repositories;

public interface IUserRepository
{
    User GetById(Guid id);
    IEnumerable<User> GetAll();
    Guid Create(User user);
    void Update(User user);
    void Delete(Guid id);
}