using FluentValidation;
using Task2.Models.Domain;

namespace Task2.Utils.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(u => u.Login)
            .NotEmpty().WithMessage("Login is required!")
            .Matches(@"^(?=.{8,}$)[^@]+@[^@]+$").WithMessage("Login must be minimum 8 symbols and contain '@' (e.g., user@dev).");

        RuleFor(u => u.Balance)
            .GreaterThanOrEqualTo(0).WithMessage("Balance can't be negative!");

        RuleFor(u => u.BlockedDeposit)
            .GreaterThanOrEqualTo(0).WithMessage("Blocked deposit can't be negative!");
    }
}