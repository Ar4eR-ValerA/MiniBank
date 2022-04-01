using FluentValidation;

namespace MiniBank.Core.Domain.Accounts.Validators;

public class AccountValidator : AbstractValidator<Account>
{
    public AccountValidator()
    {
        RuleFor(a => a.Balance)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Balance of account must be greater than or equal to 0");
    }
}