using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Common.Models;
using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Features.Customers.Queries.GetCustomers;

public sealed record GetCustomerByIdQuery(Guid CustomerId) : ICacheQuery<Result<CustomerDto>>
{
    public string Cachekey => $"customers_{CustomerId}";

    public string[] Tags => ["customer"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}