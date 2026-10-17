using FluentValidation;
using Task2.Models.Domain;

namespace Task2.Utils.Validators;

public class RentalValidator : AbstractValidator<Rental>
{
    public RentalValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.ScooterId)
            .NotEmpty().WithMessage("Scooter ID is required.");

        RuleFor(x => x.TotalCost)
            .GreaterThanOrEqualTo(0).WithMessage("Cost can't be negative.");

        RuleFor(x => x.EndTime)
            .GreaterThanOrEqualTo(x => x.StartTime)
            .When(x => x.EndTime.HasValue)
            .WithMessage("End time can't be sooner than start time.");
    }
}