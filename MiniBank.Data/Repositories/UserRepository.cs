using MiniBank.Core.Entities;
using MiniBank.Core.Repositories;

namespace MiniBank.Data.Repositories;

public class UserRepository : IUserRepository
{
    public User GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<User> GetAll()
    {
        throw new NotImplementedException();
    }

    public Guid CreateUser(User user)
    {
        throw new NotImplementedException();
    }

    public void UpdateUser(User user)
    {
        throw new NotImplementedException();
    }

    public void DeleteUser(Guid id)
    {
        throw new NotImplementedException();
    }
}