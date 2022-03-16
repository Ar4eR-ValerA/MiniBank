using MiniBank.Core.Entities;
using MiniBank.Core.Repositories;
using MiniBank.Core.Tools;
using MiniBank.Data.Repositories.DbModels;

namespace MiniBank.Data.Repositories;

public class UserRepository : IUserRepository
{
    private static readonly List<UserDbModel> Users = new();

    public User GetUserById(Guid id)
    {
        var userDbModel = Users.FirstOrDefault(u => u.Id == id);

        if (userDbModel is null)
        {
            throw new NotFoundException("There is no user with such id");
        }

        return new User
        {
            Id = userDbModel.Id,
            Login = userDbModel.Login,
            Email = userDbModel.Email,
            AccountsAmount = userDbModel.AccountsAmount
        };
    }

    public IEnumerable<User> GetAllUsers()
    {
        return Users.Select(u => new User
        {
            Id = u.Id, 
            Login = u.Login, 
            Email = u.Email, 
            AccountsAmount = u.AccountsAmount
        });
    }

    public bool Contains(Guid id)
    {
        var user = Users.FirstOrDefault(u => u.Id == id);

        return user is not null;
    }

    public Guid CreateUser(User user)
    {
        var userDbModel = new UserDbModel
        {
            Id = Guid.NewGuid(),
            Login = user.Login,
            Email = user.Email,
            AccountsAmount = 0
        };

        Users.Add(userDbModel);

        return userDbModel.Id;
    }

    public void UpdateUser(User user)
    {
        var userDbModel = Users.FirstOrDefault(u => u.Id == user.Id);

        if (userDbModel is null)
        {
            throw new NotFoundException("There is no such user");
        }

        userDbModel.Login = user.Login;
        userDbModel.Email = user.Email;
    }

    public void DeleteUser(Guid id)
    {
        var userDbModel = Users.FirstOrDefault(u => u.Id == id);

        if (userDbModel is null)
        {
            throw new NotFoundException("There is no such user");
        }

        Users.Remove(userDbModel);
    }
}