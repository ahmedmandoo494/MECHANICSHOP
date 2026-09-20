namespace MechanicShop.Application.Features.Customers.Commands.CreateCustomer;

using System.Text.RegularExpressions;
using FluentValidation;

public sealed class CreateCustomerCommandValidator
    : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Customer name is required.")
            .MaximumLength(100)
            .WithMessage("Customer name cannot exceed 100 characters.");

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Customer email is invalid.")
            .MaximumLength(100)
            .WithMessage("Customer email cannot exceed 150 characters.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Customer phone number is required.")
            .Matches(@"^\+?\d{7,15}$").WithMessage("Phone number must be 7-15 digits and may start with +.");


        RuleFor(x => x.Vehicles)
            .NotNull()
            .WithMessage("Vehicles cannot be null.")
            .Must(v => v.Count > 0).WithMessage("At least on vehicle is required.");

        RuleForEach(x => x.Vehicles)
            .SetValidator(new CreateVehicleCommandValidator());
    }
}
