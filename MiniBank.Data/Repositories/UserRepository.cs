using MiniBank.Core.Entities;
using MiniBank.Core.Repositories;
using MiniBank.Core.Tools;
using MiniBank.Data.Repositories.DbModels;

namespace MiniBank.Data.Repositories;

public class UserRepository : IUserRepository
{
    private static readonly List<UserDbModel> Users = new();

    private UserDbModel GetUserDbModelById(Guid id)
    {
        return Users.FirstOrDefault(u => u.Id == id);
    }
    
    public User GetUserById(Guid id)
    {
        var userDbModel = GetUserDbModelById(id);

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
        var userDbModel = GetUserDbModelById(user.Id);

        if (userDbModel is null)
        {
            throw new NotFoundException("There is no such user");
        }

        userDbModel.Login = user.Login;
        userDbModel.Email = user.Email;
        userDbModel.AccountsAmount = user.AccountsAmount;
    }

    public void DeleteUser(Guid id)
    {
        var userDbModel = GetUserDbModelById(id);

        if (userDbModel is null)
        {
            throw new NotFoundException("There is no such user");
        }

        Users.Remove(userDbModel);
    }
}