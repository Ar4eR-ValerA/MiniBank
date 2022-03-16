using MiniBank.Core.Entities;
using MiniBank.Core.Repositories;
using MiniBank.Core.Services.Interfaces;
using MiniBank.Core.Tools;

namespace MiniBank.Core.Services;

public class UserService : IUserService
{
    private IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public User GetById(Guid id)
    {
        return _userRepository.GetUserById(id);
    }

    public IEnumerable<User> GetAll()
    {
        return _userRepository.GetAllUsers();
    }

    public Guid CreateUser(User user)
    {
        return _userRepository.CreateUser(user);
    }

    public void UpdateUser(User user)
    {
        _userRepository.UpdateUser(user);
    }

    public void DeleteUser(Guid id)
    {
        var user = GetById(id);

        if (user.AccountsAmount > 0)
        {
            throw new UserFriendlyException("This user has not closed accounts");
        }
        
        _userRepository.DeleteUser(id);
    }
}