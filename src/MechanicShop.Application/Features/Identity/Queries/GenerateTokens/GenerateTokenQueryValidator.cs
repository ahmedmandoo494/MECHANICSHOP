using FluentValidation;

namespace MechanicShop.Application.Features.Identity.Queries.GenerateToken;

public sealed class GenerateTokenQueryValidator : AbstractValidator<GenerateTokenQuery>
{
    public GenerateTokenQueryValidator()
    {
        RuleFor(t => t.Email)
        .NotNull().NotEmpty()
        .WithErrorCode("Email_Null_Or_Empty")
        .WithMessage("Email cannot be null or empty");

        RuleFor(t => t.Password)
        .NotNull().NotEmpty()
        .WithErrorCode("Password_Null_Or_Empty")
        .WithMessage("Password cannot be null or empty");
    }
}