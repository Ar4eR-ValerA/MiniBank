using MiniBank.Core.Entities;
using MiniBank.Core.Repositories;
using MiniBank.Core.Services.Interfaces;
using MiniBank.Core.Tools;

namespace MiniBank.Core.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IAccountRepository _accountRepository;

    public UserService(IUserRepository userRepository, IAccountRepository accountRepository)
    {
        _userRepository = userRepository;
        _accountRepository = accountRepository;
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
        user.Id = Guid.NewGuid();

        return _userRepository.CreateUser(user);
    }

    public void UpdateUser(User user)
    {
        _userRepository.UpdateUser(user);
    }

    public void DeleteUser(Guid id)
    {
        if (_accountRepository.HasUserLinkedAccounts(id))
        {
            throw new UserFriendlyException("You can't delete user with linked accounts");
        }
        
        _userRepository.DeleteUser(id);
    }
}