using FluentValidation;
using Task2.Models.DTO.Business;

namespace Task2.Utils.Validators;

public class StartRentalDtoValidator : AbstractValidator<StartRentalDto>
{
    public StartRentalDtoValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required to start a rental.");

        RuleFor(x => x.ScooterId)
            .NotEmpty().WithMessage("Scooter ID is required to start a rental.");
    }
}

public class FinishRentalDtoValidator : AbstractValidator<FinishRentalDto>
{
    public FinishRentalDtoValidator()
    {
        RuleFor(x => x.RentalId)
            .NotEmpty().WithMessage("Rental ID is required.");
    }
}

public class CancelRentalDtoValidator : AbstractValidator<CancelRentalDto>
{
    public CancelRentalDtoValidator()
    {
        RuleFor(x => x.RentalId)
            .NotEmpty().WithMessage("Rental ID is required.");

        RuleFor(x => x.CancellationReason)
            .MaximumLength(200).WithMessage("Cancellation reason cannot exceed 200 characters.")
            .When(x => !string.IsNullOrEmpty(x.CancellationReason));
    }
}
