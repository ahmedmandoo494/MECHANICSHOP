using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders.Enums;
using MechanicShop.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace MechanicShop.Infrastructure.Services;

public class WorkOrderPolicy(IOptions<AppSettings> options, IAppDbContext context) : IWorkOrderPolicy
{
    private readonly AppSettings _appSettings = options.Value;
    private readonly IAppDbContext _context = context;

    public async Task<Result<Success>> CheckSpotAvailabilityResult(Spots spot, DateTimeOffset startAt, DateTimeOffset endAt, Guid? excludeWorkOrderId = null, CancellationToken ct = default)
    {
        var isOccupied = await _context.WorkOrders.AnyAsync(w => 
            w.Spot == spot &&
            w.StartAtUtc < endAt &&
            w.EndAtUtc > startAt &&
            (!excludeWorkOrderId.HasValue || w.Id != excludeWorkOrderId.Value)
        );
       return isOccupied ?
            Error.Conflict("MechaincShop_Spot_Full","The Selected time spot is unavailable for the requested services.") :
            Result.Success;
    }

    public async Task<bool> IsLaborOccupied(Guid LaborId, Guid excludeWorkOrderId, DateTimeOffset startAt, DateTimeOffset endAt)
    {
        var isOccupied = await _context.WorkOrders.AnyAsync(w => 
            w.LaborId == LaborId &&
            w.StartAtUtc < endAt &&
            w.EndAtUtc > startAt &&
            w.Id != excludeWorkOrderId
        );
       return isOccupied;
    }

    public bool IsOutsideOperatingHours(DateTimeOffset startAt, DateTimeOffset endAt)
    {
        var openAt = new DateTimeOffset(
            startAt.Date,
            startAt.Offset)
            .Add(_appSettings.OpeningTime.ToTimeSpan());

        var closeAt = new DateTimeOffset(
            startAt.Date,
            startAt.Offset)
            .Add(_appSettings.ClosingTime.ToTimeSpan());
        return startAt < openAt || endAt > closeAt;

    }

    public async Task<bool> IsVehicleAlreadyScheduled(Guid vehicleId, DateTimeOffset startAt, DateTimeOffset endAt, Guid excludeWorkOrderId)
    {
        var isScheduled = await _context.WorkOrders.AnyAsync(w => 
            w.VehicleId == vehicleId &&
            w.StartAtUtc < endAt &&
            w.EndAtUtc > startAt &&
            w.Id != excludeWorkOrderId
        );
       return isScheduled;
    }

    public Result<Success> ValidateMinimumRequirement(DateTimeOffset startAt, DateTimeOffset endAt)
    {
        if((endAt - startAt) < TimeSpan.FromMinutes(_appSettings.MinimumAppointmentDurationInMinutes))
        {
            return Error.Conflict("WorkOrder_TooShort",$"WorkOrder duriation must be at least{_appSettings.MinimumAppointmentDurationInMinutes} minutes.");
        }
        return Result.Success;
    }

}