using FluentValidation;
using MiniBank.Core.Domain.Accounts.Repositories;

namespace MiniBank.Core.Domain.Transactions.Validators;

public class TransactionValidator : AbstractValidator<Transaction>
{
    public TransactionValidator(IAccountRepository accountRepository)
    {
        RuleFor(t => t)
            .Must(transaction => transaction.FromAccountId != transaction.ToAccountId)
            .WithMessage("Accounts must be different");
        
        RuleFor(t => t.Amount)
            .GreaterThan(0)
            .WithMessage("Transaction's amount must be positive");

        RuleFor(t => t.FromAccountId)
            .Must(fromAccountId => accountRepository.GetById(fromAccountId).IsActive)
            .WithMessage("Sender is not active");

        RuleFor(t => t.ToAccountId)
            .Must(toAccountId => accountRepository.GetById(toAccountId).IsActive)
            .WithMessage("Receiver is not active");

        RuleFor(t => t)
            .Must(transaction =>
            {
                var fromAccount = accountRepository.GetById(transaction.FromAccountId);
                return fromAccount.Balance - (transaction.Amount + transaction.Commission) >= 0;
            })
            .WithMessage("Sender's balance will be negative after operation");
    }
}