using System;
using FluentValidation;
using SimRacingDashboard.Api.Dtos.Auth;

namespace SimRacingDashboard.Api.Validator.Auth;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    private static string[] CommonPasswords;

    public RegisterRequestValidator()
    {
        if (CommonPasswords == null)
            CommonPasswords = File.ReadAllLines("CommonPasswords.txt");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
            .MaximumLength(50).WithMessage("Username must not exceed 50 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .MaximumLength(128).WithMessage("Password must not exceed 128 characters.");
    }
}
