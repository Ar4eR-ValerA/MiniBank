using MiniBank.Core.Entities;
using MiniBank.Core.Entities.Builders;
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

    public Guid CreateUser(UserBuilder userBuilder)
    {
        var user = userBuilder.Build();

        return _userRepository.CreateUser(user);
    }

    public void UpdateUser(Guid id, UserBuilder userBuilder)
    {
        var user = _userRepository.GetUserById(id);

        user.Login = userBuilder.Login;
        user.Email = userBuilder.Email;
        
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