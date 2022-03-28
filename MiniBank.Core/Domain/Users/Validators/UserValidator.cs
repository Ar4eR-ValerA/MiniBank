using FluentValidation;
using MiniBank.Core.Domain.Users.Repositories;

namespace MiniBank.Core.Domain.Users.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator(IUserRepository userRepository)
    {
        RuleFor(u => u.Login).NotEmpty().WithMessage("Login must be not empty");
        
        RuleFor(u => u.Login.Length)
            .LessThanOrEqualTo(20)
            .WithMessage("Login's length must be less than or equal to 20");
        
        RuleFor(u => u.Login)
            .Must(login => !userRepository.IsLoginExists(login))
            .WithMessage(u => $"There is another user with this login: {u.Login}");
    }
}