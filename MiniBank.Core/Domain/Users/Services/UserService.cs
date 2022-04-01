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

    public Task<User> GetById(Guid id)
    {
        return _userRepository.GetById(id);
    }

    public Task<IReadOnlyList<User>> GetAll()
    {
        return _userRepository.GetAll();
    }

    public async Task<Guid> Create(User user)
    {
        await _userValidator.ValidateAndThrowAsync(user);

        if (await _userRepository.IsLoginExists(user.Login))
        {
            throw new UserFriendlyException($"There is another user with this login: {user.Login}");
        }

        var userId = Guid.NewGuid();
        user.Id = userId;

        await _userRepository.Create(user);
        await _unitOfWork.SaveChangesAsync();

        return userId;
    }

    public async Task Update(User user)
    {
        await _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        if (!await _userRepository.IsExist(id))
        {
            throw new UserFriendlyException($"There is no user with such id: {id}");
        }

        if (await _accountRepository.HasUserLinkedAccounts(id))
        {
            throw new UserFriendlyException("You can't delete user with linked accounts");
        }

        await _userRepository.Delete(id);
        await _unitOfWork.SaveChangesAsync();
    }
}