using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Vehicles;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Customers.Commands.UpdateCustomer;

public sealed class UpdateCustomerCommandHandler(IAppDbContext context,ILogger<UpdateCustomerCommandHandler>logger,HybridCache cache) : IRequestHandler<UpdateCustomerCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<UpdateCustomerCommandHandler> _logger = logger;
    private readonly HybridCache _cache = cache;

    public async Task<Result<Updated>> Handle(UpdateCustomerCommand command, CancellationToken ct)
    {
        var customer = await _context.Customers.Include(c => c.Vehicles).FirstOrDefaultAsync(c => c.Id == command.CustomerId,ct);
        if(customer is null)
        {
            _logger.LogInformation("Customer {customerId} not found for Update",command.CustomerId);
            return ApplicationErrors.CustomerNotFound;
        }
        var validateVehicles = new List<Vehicle>();
        foreach (var v in command.Vehicles)
        {
            var vehicleId = v.VehicleId ?? Guid.NewGuid();
            var vehicleResult = Vehicle.Create(vehicleId,v.Make,v.Model,v.Year,v.LicensePlate);
            if (vehicleResult.IsError)
            {
                return vehicleResult.Errors!;
            }
            validateVehicles.Add(vehicleResult.Value);
        }

        var updateCustomerResult = customer.Update(command.Name,command.PhoneNumber,command.Email);
        if (updateCustomerResult.IsError)
        {
            return updateCustomerResult.Errors!;
        }

        var upsertPartsResult = customer.UpsertVehicles(validateVehicles);
        if (upsertPartsResult.IsError)
        {
            return upsertPartsResult.Errors!;
        }
        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("customer",ct);
        return Result.Updated;
    }
}