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

    public Task<User> GetById(Guid id, CancellationToken cancellationToken)
    {
        return _userRepository.GetById(id, cancellationToken);
    }

    public Task<IReadOnlyList<User>> GetAll(CancellationToken cancellationToken)
    {
        return _userRepository.GetAll(cancellationToken);
    }

    public async Task<Guid> Create(User user, CancellationToken cancellationToken)
    {
        await _userValidator.ValidateAndThrowAsync(user, cancellationToken);

        if (await _userRepository.IsLoginExists(user.Login, cancellationToken))
        {
            throw new UserFriendlyException($"There is another user with this login: {user.Login}");
        }

        var userId = Guid.NewGuid();
        user.Id = userId;

        await _userRepository.Create(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return userId;
    }

    public async Task Update(User user, CancellationToken cancellationToken)
    {
        await _userRepository.Update(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task Delete(Guid id, CancellationToken cancellationToken)
    {
        if (!await _userRepository.IsExist(id, cancellationToken))
        {
            throw new UserFriendlyException($"There is no user with such id: {id}");
        }

        if (await _accountRepository.HasUserLinkedAccounts(id, cancellationToken))
        {
            throw new UserFriendlyException("You can't delete user with linked accounts");
        }

        await _userRepository.Delete(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync();
    }
}