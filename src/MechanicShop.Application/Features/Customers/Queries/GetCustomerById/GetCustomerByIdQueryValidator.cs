using FluentValidation;
using MechanicShop.Application.Features.Customers.Queries.GetCustomers;

namespace MechanicShop.Application.Features.Customers.Commands.UpdateCustomer;

public sealed class GetCustomerByIdQueryValidator : AbstractValidator<GetCustomerByIdQuery>
{
    public GetCustomerByIdQueryValidator()
    {
        RuleFor(c => c.CustomerId)
            .NotEmpty()
            .WithErrorCode("CustomerId_is_requierd")
            .WithMessage("Customer id is requierd.");
    }
}
