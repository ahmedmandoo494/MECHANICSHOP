using System.Security.AccessControl;
using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers;
using MechanicShop.Domain.Vehicles;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Customers.Commands.UpdateCustomer;

public sealed class RemoveCustomerCommandHandler(IAppDbContext context,ILogger<RemoveCustomerCommandHandler>logger,HybridCache cache) : IRequestHandler<RemoveCustomerCommand, Result<Deleted>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<RemoveCustomerCommandHandler> _logger = logger;
    private readonly HybridCache _cache = cache;

    public async Task<Result<Deleted>> Handle(RemoveCustomerCommand command, CancellationToken ct)
    {
         var customer = await _context.Customers.FindAsync([command.CustomerId],ct);
        if(customer is null)
        {
            _logger.LogInformation("Customer {customerId} not exist for Deleting",command.CustomerId);
            return ApplicationErrors.CustomerNotFound;
        }
        var hasAssocitatedWorkOrders = await _context.WorkOrders.Include(wo => wo.Vehicle)
        .Where(wo => wo.Vehicle != null)
        .AnyAsync(wo => wo.Vehicle!.CustomerId == command.CustomerId);
        if (hasAssocitatedWorkOrders)
        {
            _logger.LogInformation("Customer {customerId} cannot be deleted because they have associated work orders (past,scheduled,or in-progress).",command.CustomerId);
            return CustomerErrors.CannontDeleteCustomerWithWorkOrder;
        }
        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync(["customer"]);
        _logger.LogInformation("Customer {customerId} deleted successfuly.",command.CustomerId);


        return Result.Deleted;
    }
}