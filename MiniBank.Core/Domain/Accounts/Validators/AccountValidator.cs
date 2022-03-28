using FluentValidation;
using MiniBank.Core.Domain.Users.Repositories;

namespace MiniBank.Core.Domain.Accounts.Validators;

public class AccountValidator : AbstractValidator<Account>
{
    public AccountValidator(IUserRepository userRepository)
    {
        RuleFor(a => a.Balance)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Balance of account must be greater than or equal to 0");

        RuleFor(a => a.UserId)
            .Must(userRepository.IsExist)
            .WithMessage(a => $"There is no user with such id: {a.UserId}");
    }
}