using FluentValidation;
using Task2.Models.Domain;

namespace Task2.Utils.Validators;

public class ScooterValidator : AbstractValidator<Scooter>
{
    public ScooterValidator()
    {
        RuleFor(x => x.BatteryCharge)
            .InclusiveBetween(0, 100).WithMessage("Battery charge must be from 0 to 100%.");

        RuleFor(x => x.Tariff)
            .GreaterThanOrEqualTo(0).WithMessage("Tariff can't be negative.");

        RuleFor(x => x.Coordinate)
            .NotNull().WithMessage("Coordinates are required.");

        RuleFor(x => x.Coordinate.Latitude)
            .InclusiveBetween(-90.0, 90.0).WithMessage("Latitude must be from -90 to 90.");

        RuleFor(x => x.Coordinate.Longitude)
            .InclusiveBetween(-180.0, 180.0).WithMessage("Longitude must be from -180 to 180.");
    }
}