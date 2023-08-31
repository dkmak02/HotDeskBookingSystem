using System.Data;
using FastEndpoints;
using FluentValidation;
using Hot_desk_booking_system.Models.RequestModels;

namespace Hot_desk_booking_system.Validators;

public class BookingValidator : Validator<BookDeskRequest>
{
    public BookingValidator()
    {
        RuleFor(x => x.days)
            .NotEmpty()
            .WithMessage("Days cannot be empty")
            .GreaterThan(0)
            .WithMessage("Days must be greater than 0")
            .LessThan(8)
            .WithMessage("Days must be less than 8");
        RuleFor(x => x.from)
            .NotEmpty()
            .WithMessage("From date cannot be empty")
            .Matches("[0-9]{4}-[0-9]{2}-[0-9]{2}")
            .WithMessage("From date must be in format yyyy-mm-dd");
        
    }
}