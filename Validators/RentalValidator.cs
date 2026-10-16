using FluentValidation;
using Task2.Models;

namespace Task2.Validators;

public class RentalValidator : AbstractValidator<Rental>
{
    public RentalValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("Not valid user ID.");

        RuleFor(x => x.ScooterId)
            .GreaterThan(0).WithMessage("Not valid scooter ID.");

        RuleFor(x => x.TotalCost)
            .GreaterThanOrEqualTo(0).WithMessage("Cost can't be negative.");

        RuleFor(x => x.EndTime)
            .GreaterThanOrEqualTo(x => x.StartTime)
            .When(x => x.EndTime.HasValue)
            .WithMessage("End time ca't be sooner than start time.");
    }
}
