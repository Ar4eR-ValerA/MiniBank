using FluentValidation;

namespace MiniBank.Core.Domain.Users.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(u => u.Login).NotEmpty().WithMessage("Login must be not empty");
        
        RuleFor(u => u.Login.Length)
            .LessThanOrEqualTo(20)
            .WithMessage("Login's length must be less than or equal to 20");
    }
}