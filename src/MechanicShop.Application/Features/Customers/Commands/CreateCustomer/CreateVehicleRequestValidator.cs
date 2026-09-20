namespace MechanicShop.Application.Features.Customers.Commands.CreateCustomer;

using FluentValidation;


public sealed class CreateVehicleCommandValidator
    : AbstractValidator<CreateVehicleCommand>
{
    public CreateVehicleCommandValidator()
    {
        RuleFor(x => x.Make)
            .NotEmpty()
            .WithMessage("Vehicle make is required.")
            .MaximumLength(50)
            .WithMessage("Vehicle make cannot exceed 50 characters.");

        RuleFor(x => x.Model)
            .NotEmpty()
            .WithMessage("Vehicle model is required.")
            .MaximumLength(50)
            .WithMessage("Vehicle model cannot exceed 50 characters.");


        RuleFor(x => x.LicensePlate)
            .NotEmpty()
            .WithMessage("License plate is required.")
            .MaximumLength(10)
            .WithMessage("License plate cannot exceed 20 characters.");
    }
}