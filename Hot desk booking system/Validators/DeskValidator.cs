using FastEndpoints;
using FluentValidation;
using Hot_desk_booking_system.Models.RequestModels;

namespace Hot_desk_booking_system.Validators;

public class DeskValidator : Validator<DeskRequest>
{
    public DeskValidator()
    {
        RuleFor(x => x.deskName)
            .NotEmpty()
            .WithMessage("Desk name cannot be empty")
            .Matches("[a-zA-Z][0-9]")
            .WithMessage("Desk name must match [a-zA-Z][0-9");
        RuleFor(x => x.locationName).NotEmpty().WithMessage("Location name cannot be empty");
    }
}