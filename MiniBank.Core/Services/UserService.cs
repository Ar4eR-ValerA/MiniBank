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
        return _userRepository.GetById(id);
    }

    public IEnumerable<User> GetAll()
    {
        return _userRepository.GetAll();
    }

    public Guid Create(User user)
    {
        user.Id = Guid.NewGuid();

        return _userRepository.Create(user);
    }

    public void Update(User user)
    {
        _userRepository.Update(user);
    }

    public void Delete(Guid id)
    {
        if (_accountRepository.HasUserLinkedAccounts(id))
        {
            throw new UserFriendlyException("You can't delete user with linked accounts");
        }
        
        _userRepository.Delete(id);
    }
}