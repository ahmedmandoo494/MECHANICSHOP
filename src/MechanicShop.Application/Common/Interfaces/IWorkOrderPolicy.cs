using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders.Enums;

namespace MechanicShop.Application.Common.Interfaces;

public interface IWorkOrderPolicy
{
    bool IsOutsideOperatingHours(DateTimeOffset startAt,DateTimeOffset endAt);
    Task<Result<Success>> CheckSpotAvailabilityResult(Spots spot,DateTimeOffset startAt,DateTimeOffset endAt, Guid ?excludeWorkOrderId =null,CancellationToken ct =default);
    Result<Success> ValidateMinimumRequirement(DateTimeOffset startAt, DateTimeOffset endAt);
    Task<bool> IsLaborOccupied(Guid LaborId, Guid excludeWorkOrderId,DateTimeOffset startAt,DateTimeOffset endAt);
    Task<bool> IsVehicleAlreadyScheduled(Guid vehicleId,DateTimeOffset startAt,DateTimeOffset endAt, Guid excludeWorkOrderId);
}
