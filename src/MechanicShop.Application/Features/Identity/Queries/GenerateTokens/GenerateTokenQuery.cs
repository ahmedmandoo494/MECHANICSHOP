using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Customers.Queries.GetCustomers;
using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.Identity.Queries.GenerateToken;

public sealed record GenerateTokenQuery(string Email,string Password):IRequest<Result<TokenResponse>>;
