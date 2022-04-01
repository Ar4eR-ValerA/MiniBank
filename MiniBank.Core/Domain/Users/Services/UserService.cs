using FluentValidation;
using MiniBank.Core.Domain.Accounts.Repositories;
using MiniBank.Core.Domain.Users.Repositories;
using MiniBank.Core.Tools;

namespace MiniBank.Core.Domain.Users.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IValidator<User> _userValidator;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(
        IUserRepository userRepository,
        IAccountRepository accountRepository,
        IValidator<User> userValidator,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _accountRepository = accountRepository;
        _userValidator = userValidator;
        _unitOfWork = unitOfWork;
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
        _userValidator.ValidateAndThrow(user);

        var userId = Guid.NewGuid();
        user.Id = userId;

        _userRepository.Create(user);
        _unitOfWork.SaveChanges();

        return userId;
    }

    public void Update(User user)
    {
        _userRepository.Update(user);
        _unitOfWork.SaveChanges();
    }

    public void Delete(Guid id)
    {
        if (!_userRepository.IsExist(id))
        {
            throw new UserFriendlyException($"There is no user with such id: {id}");
        }

        if (_accountRepository.HasUserLinkedAccounts(id))
        {
            throw new UserFriendlyException("You can't delete user with linked accounts");
        }

        _userRepository.Delete(id);
        _unitOfWork.SaveChanges();
    }
}