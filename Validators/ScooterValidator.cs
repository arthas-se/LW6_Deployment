using FluentValidation;
using Task2.Models.Domain;

namespace Task2.Validators;

public class ScooterValidator : AbstractValidator<Scooter>
{
    public ScooterValidator()
    {
        RuleFor(x => x.BatteryCharge)
            .InclusiveBetween(0, 100).WithMessage("Battery charge from 0 to 100%.");

        RuleFor(x => x.Tariff)
            .GreaterThanOrEqualTo(0).WithMessage("Tariff can't be negative.");

        RuleFor(x => x.Coordinate)
            .NotNull().WithMessage("Coordinates is requaired.");

        RuleFor(x => x.Coordinate.Latitude)
            .InclusiveBetween(-90.0, 90.0).WithMessage("From -90 to 90.");

        RuleFor(x => x.Coordinate.Longitude)
            .InclusiveBetween(-180.0, 180.0).WithMessage("From -180 to 180.");
    }
}
