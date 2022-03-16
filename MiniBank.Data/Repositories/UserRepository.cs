﻿using MiniBank.Core.Entities;
using MiniBank.Core.Repositories;
using MiniBank.Data.Repositories.DbModels;

namespace MiniBank.Data.Repositories;

public class UserRepository : IUserRepository
{
    private List<UserDbModel> _users;

    public UserRepository()
    {
        _users = new List<UserDbModel>();
    }

    public User GetUserById(Guid id)
    {
        var userDbModel = _users.FirstOrDefault(u => u.Id == id);

        if (userDbModel is null)
        {
            throw new Exception("There is no user with such id");
        }

        return new User(userDbModel.Id, userDbModel.Login, userDbModel.Email);
    }

    public IEnumerable<User> GetAllUsers()
    {
        return _users.Select(u => new User(u.Id, u.Login, u.Email));
    }

    public Guid CreateUser(User user)
    {
        var userDbModel = new UserDbModel(Guid.NewGuid(), user.Login, user.Email);

        _users.Add(userDbModel);

        return userDbModel.Id;
    }

    public void UpdateUser(User user)
    {
        var userDbModel = _users.FirstOrDefault(u => u.Id == user.Id);

        if (userDbModel is null)
        {
            throw new Exception("There is no such user");
        }

        userDbModel.Login = user.Login;
        userDbModel.Email = user.Email;
    }

    public void DeleteUser(Guid id)
    {
        var userDbModel = _users.FirstOrDefault(u => u.Id == id);

        if (userDbModel is null)
        {
            throw new Exception("There is no such user");
        }

        _users.Remove(userDbModel);
    }
}