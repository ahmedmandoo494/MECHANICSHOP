using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Common.Models;
using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Application.Features.Customers.Mapper;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Customers.Queries.GetCustomers;

public sealed class GetCustomerByIdQueryHandler(IAppDbContext context, ILogger<GetCustomerByIdQueryHandler> logger) : IRequestHandler<GetCustomerByIdQuery,Result<CustomerDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<GetCustomerByIdQueryHandler> _logger = logger;

    public async Task<Result<CustomerDto>> Handle(GetCustomerByIdQuery query, CancellationToken ct)
    {
        var customer = await _context.Customers.Include(c => c.Vehicles)
                                                .AsNoTracking()
                                                .FirstOrDefaultAsync(c => c.Id == query.CustomerId);
        if(customer is null)
        {
            _logger.LogInformation("Customer {customerId} not found",query.CustomerId);
            return ApplicationErrors.CustomerNotFound;
        }
       return customer.ToDto();
    }
}