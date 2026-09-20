
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Application.Features.Customers.Mapper;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers;
using MechanicShop.Domain.Vehicles;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Customers.Commands.CreateCustomer;

public sealed class CreateCustomerCommandHandler(
    IAppDbContext context,
    ILogger<CreateCustomerCommandHandler> logger,
    HybridCache cache
    )
    :IRequestHandler<CreateCustomerCommand, Result<CustomerDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<CreateCustomerCommandHandler> _logger = logger;
    private readonly HybridCache _cache = cache;

    public async Task<Result<CustomerDto>> Handle(CreateCustomerCommand command, CancellationToken ct)
    {
        var email = command.Email.Trim().ToLower();
        var exist = await _context.Customers.AnyAsync(c => c.Email!.ToLower() == email,ct);
        if (exist)
        {
            return CustomerErrors.CustomerExists;
        }

        var vehicles = new List<Vehicle>();

        foreach (var v in command.Vehicles)
        {
            var vehicleResult =  Vehicle.Create(Guid.NewGuid(),
            v.Make,
            v.Model,
            v.Year,
            v.LicensePlate);

            if(vehicleResult.IsError)
                return vehicleResult.Errors!;

            vehicles.Add(vehicleResult.Value);
        }


        var customerResult = Customer.Create
        (Guid.NewGuid(),
        command.Name.Trim(),
        command.PhoneNumber.Trim(),
        command.Email,
        vehicles
        );

        if(customerResult.IsError)
            return customerResult.Errors!;


        var customer = customerResult.Value;

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("Customer Created Successfuly. Id:{customerId}",customer.Id);
        await _cache.RemoveByTagAsync("customer",ct);
        return customer.ToDto();
    }
}