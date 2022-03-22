using MiniBank.Core.Domain.Users;
using MiniBank.Core.Domain.Users.Repositories;
using MiniBank.Core.Tools;

namespace MiniBank.Data.Users.Repositories;

public class UserRepository : IUserRepository
{
    private static readonly List<UserDbModel> Users = new();

    private UserDbModel GetUserDbModelById(Guid id)
    {
        return Users.FirstOrDefault(u => u.Id == id);
    }

    public bool IsExist(Guid id)
    {
        return Users.Exists(u => u.Id == id);
    }

    public User GetById(Guid id)
    {
        var userDbModel = GetUserDbModelById(id);

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
        return Users.Select(u => new User
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

        Users.Add(userDbModel);

        return userDbModel.Id;
    }

    public void Update(User user)
    {
        var userDbModel = GetUserDbModelById(user.Id);

        if (userDbModel is null)
        {
            throw new ObjectNotFoundException($"There is no user with such id: {user.Id}");
        }

        userDbModel.Login = user.Login;
        userDbModel.Email = user.Email;
    }

    public void Delete(Guid id)
    {
        var userDbModel = GetUserDbModelById(id);

        if (userDbModel is null)
        {
            throw new ObjectNotFoundException($"There is no user with such id: {id}");
        }

        Users.Remove(userDbModel);
    }
}