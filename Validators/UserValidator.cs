using FluentValidation;
using Task2.Models.Domain;

namespace Task2.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(u => u.Login)
            .NotEmpty().WithMessage("Login is required!")
            .Matches(@"^(?=.{8,}$)[^@]+@[^@]+$").WithMessage("Login must be minimum 8 symbols and with '@' inside (like, user@dev).");

        RuleFor(u => u.Balance)
            .GreaterThan(-1).WithMessage("Balance can't be negative!");

        RuleFor(u => u.BlockedDeposit)
            .GreaterThan(-1).WithMessage("Blocked deposit can't be negative!");
    }
}
