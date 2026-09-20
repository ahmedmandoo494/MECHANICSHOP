using FluentValidation;

namespace MechanicShop.Application.Features.Customers.Commands.UpdateCustomer;

public sealed class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(c => c.CustomerId)
            .NotEmpty();
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100);
        RuleFor(c => c.Email)
            .EmailAddress().WithMessage("Invalid email")
            .MaximumLength(100);
        RuleFor(c => c.PhoneNumber)
            .NotEmpty()
            .WithMessage("Customer phone number is required.")
            .Matches(@"^\+?\d{7,15}$").WithMessage("Phone number must be 7-15 digits and may start with +.");


        RuleFor(c => c.Vehicles)
            .NotNull()
            .WithMessage("Vehicles cannot be null.")
            .Must(v => v.Count > 0).WithMessage("At least on vehicle is required.");

        RuleForEach(c=> c.Vehicles)
        .SetValidator(new UpdateVehicleCommandValidator());
    }
}
