using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Common.Models;
using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Application.Features.Customers.Mapper;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MechanicShop.Application.Features.Customers.Queries.GetCustomers;

public sealed class GetCustomersQueryHandler(IAppDbContext context) : IRequestHandler<GetCustomersQuery,Result<List<CustomerDto>>>
{
    public async Task<Result<List<CustomerDto>>> Handle(GetCustomersQuery request, CancellationToken ct)
    {
        var customer = await context.Customers.Include(c => c.Vehicles).AsNoTracking().ToListAsync(ct);
       return customer.ToDtos();
    }
}