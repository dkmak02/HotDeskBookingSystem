using FastEndpoints;
using FluentValidation;
using Hot_desk_booking_system.Models.RequestModels;

namespace Hot_desk_booking_system.Validators;

public class SignUpValidator : Validator<SignUpRequest>
{
    public SignUpValidator() {
        RuleFor(x => x.email)
            .NotEmpty()
            .WithMessage("Email can not be empty")
            .EmailAddress()
            .WithMessage("Wrong email format");
        RuleFor(x => x.password.Equals(x.passwordConfirm))
            .NotEmpty()
            .WithMessage("Passwords are not the same");
        RuleFor(x => x.password)
            .NotEmpty()
            .WithMessage("Password can not be empty")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long")
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]")
            .WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one number")
            .Matches("[^a-zA-Z0-9]")
            .WithMessage("Password must contain at least one special character");
        RuleFor(x => x.firstName)
            .NotEmpty()
            .WithMessage("First name can not be empty")
            .Matches("[a-zA-Z]")
            .WithMessage("First name must contain only letters");
        RuleFor(x => x.lastName)
            .NotEmpty()
            .WithMessage("Last name can not be empty")
            .Matches("[a-zA-Z]")
            .WithMessage("Last name must contain only letters");
    }
}