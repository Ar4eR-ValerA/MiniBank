using Microsoft.EntityFrameworkCore;
using MiniBank.Core.Domain.Users;
using MiniBank.Core.Domain.Users.Repositories;
using MiniBank.Core.Tools;
using MiniBank.Data.Contexts;

namespace MiniBank.Data.Users.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MiniBankContext _context;

    public UserRepository(MiniBankContext context)
    {
        _context = context;
    }

    public bool IsExist(Guid id)
    {
        return _context.Users.AsNoTracking().Any(u => u.Id == id);
    }

    public bool IsLoginExists(string login)
    {
        return _context.Users.Any(u => u.Login == login);
    }

    public User GetById(Guid id)
    {
        var userDbModel = _context.Users.AsNoTracking().FirstOrDefault(u => u.Id == id);

        if (userDbModel is null)
        {
            throw new ObjectNotFoundException($"There is no user with such id: {id}");
        }

        return new User
        {
            Id = userDbModel.Id,
            Login = userDbModel.Login,
            Email = userDbModel.Email
        };
    }

    public IEnumerable<User> GetAll()
    {
        return _context.Users.AsNoTracking().Select(u => new User
        {
            Id = u.Id,
            Login = u.Login,
            Email = u.Email
        });
    }

    public Guid Create(User user)
    {
        var userDbModel = new UserDbModel
        {
            Id = Guid.NewGuid(),
            Login = user.Login,
            Email = user.Email
        };

        _context.Users.Add(userDbModel);

        return userDbModel.Id;
    }

    public void Update(User user)
    {
        var userDbModel = _context.Users.FirstOrDefault(u => u.Id == user.Id);

        if (userDbModel is null)
        {
            throw new ObjectNotFoundException($"There is no user with such id: {user.Id}");
        }

        userDbModel.Login = user.Login;
        userDbModel.Email = user.Email;
    }

    public void Delete(Guid id)
    {
        var userDbModel = _context.Users.FirstOrDefault(u => u.Id == id);

        if (userDbModel is null)
        {
            throw new ObjectNotFoundException($"There is no user with such id: {id}");
        }

        _context.Users.Remove(userDbModel);
    }
}