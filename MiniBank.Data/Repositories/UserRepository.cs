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

        return new User(userDbModel.Id, userDbModel.Login, userDbModel.Email, userDbModel.AccountsAmount);
    }

    public IEnumerable<User> GetAllUsers()
    {
        return Users.Select(u => new User(u.Id, u.Login, u.Email, u.AccountsAmount));
    }

    public Guid CreateUser(User user)
    {
        var userDbModel = new UserDbModel(Guid.NewGuid(), user.Login, user.Email, user.AccountsAmount);

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