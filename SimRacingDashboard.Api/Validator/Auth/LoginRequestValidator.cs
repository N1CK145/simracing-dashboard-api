using System;
using FluentValidation;
using SimRacingDashboard.Api.Dtos.Auth;

namespace SimRacingDashboard.Api.Validator.Auth;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(r => r.Password)
            .NotEmpty().WithMessage("Password cannot be empty.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");

        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("Password cannot be empty.")
            .EmailAddress().WithMessage("Invalid email format.");
    }
}
