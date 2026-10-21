using FluentValidation;
using Task2.Models.DTO;

namespace Task2.Utils.Validators;

public class UserPatchDtoValidator : AbstractValidator<UserPatchDto>
{
    public UserPatchDtoValidator()
    {
        RuleFor(u => u.Login)
            .NotEmpty().WithMessage("Login cannot be empty.")
            .Matches(@"^(?=.{8,}$)[^@]+@[^@]+$").WithMessage("Login must be minimum 8 symbols and contain '@' (e.g., user@dev).")
            .When(u => u.Login != null);

        RuleFor(u => u.Balance)
            .GreaterThanOrEqualTo(0).WithMessage("Balance can't be negative!")
            .When(u => u.Balance.HasValue);

        RuleFor(u => u.BlockedDeposit)
            .GreaterThanOrEqualTo(0).WithMessage("Blocked deposit can't be negative!")
            .When(u => u.BlockedDeposit.HasValue);
    }
}

public class ScooterPatchDtoValidator : AbstractValidator<ScooterPatchDto>
{
    public ScooterPatchDtoValidator()
    {
        RuleFor(x => x.BatteryCharge)
            .InclusiveBetween(0, 100).WithMessage("Battery charge must be from 0 to 100%.")
            .When(x => x.BatteryCharge.HasValue);

        RuleFor(x => x.Tariff)
            .GreaterThanOrEqualTo(0).WithMessage("Tariff can't be negative.")
            .When(x => x.Tariff.HasValue);

        When(x => x.Coordinate != null, () =>
        {
            RuleFor(x => x.Coordinate!.Latitude)
                .InclusiveBetween(-90.0, 90.0).WithMessage("Latitude must be from -90 to 90.");

            RuleFor(x => x.Coordinate!.Longitude)
                .InclusiveBetween(-180.0, 180.0).WithMessage("Longitude must be from -180 to 180.");
        });
    }
}

public class RentalPatchDtoValidator : AbstractValidator<RentalPatchDto>
{
    public RentalPatchDtoValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.")
            .When(x => x.UserId != null);

        RuleFor(x => x.ScooterId)
            .NotEmpty().WithMessage("Scooter ID is required.")
            .When(x => x.ScooterId != null);

        RuleFor(x => x.TotalCost)
            .GreaterThanOrEqualTo(0).WithMessage("Cost can't be negative.")
            .When(x => x.TotalCost.HasValue);

        RuleFor(x => x.EndTime)
            .GreaterThanOrEqualTo(x => x.StartTime)
            .When(x => x.EndTime.HasValue && x.StartTime.HasValue)
            .WithMessage("End time can't be sooner than start time.");
    }
}