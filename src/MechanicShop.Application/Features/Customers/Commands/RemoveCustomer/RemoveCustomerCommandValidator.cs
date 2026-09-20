using FluentValidation;

namespace MechanicShop.Application.Features.Customers.Commands.UpdateCustomer;

public sealed class RemoveCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public RemoveCustomerCommandValidator()
    {
        RuleFor(c => c.CustomerId)
            .NotEmpty().WithMessage("Customer id is requierd.");
    }
}
