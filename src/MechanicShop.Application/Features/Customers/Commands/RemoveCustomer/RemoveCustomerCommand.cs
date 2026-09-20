using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.Customers.Commands.UpdateCustomer;

public sealed record RemoveCustomerCommand(Guid CustomerId):IRequest<Result<Deleted>>;
