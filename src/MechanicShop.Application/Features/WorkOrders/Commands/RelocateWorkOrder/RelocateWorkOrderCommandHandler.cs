using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders;
using MechanicShop.Domain.WorkOrders.Enums;
using MechanicShop.Domain.WorkOrders.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;


namespace MechanicShop.Application.Features.WorkOrders.Commands.RelocateWorkOrder;

public sealed class RelocateWorkOrderCommandHandler(
    IAppDbContext context,
    ILogger<RelocateWorkOrderCommandHandler> logger,
    HybridCache cache,
    IWorkOrderPolicy workOrderPolicy,
    TimeProvider timeProvider) : IRequestHandler<RelocateWorkOrderCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<RelocateWorkOrderCommandHandler> _logger = logger;
    private readonly HybridCache _cache = cache;
    private readonly IWorkOrderPolicy _workOrderPolicy = workOrderPolicy;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<Result<Updated>> Handle(RelocateWorkOrderCommand command, CancellationToken ct)
    {
        var workOrder = await _context.WorkOrders
        .Include(w => w.RepairTasks)
        .Include(w => w.Labor)
        .Include(w => w.Vehicle)
        .FirstOrDefaultAsync(w => w.Id == command.WorkOrderId,ct);
        if(workOrder is null)
        {
            _logger.LogError("WorkOrder with Id '{workOrderId} does not exist.'",command.WorkOrderId);
            return ApplicationErrors.WorkOrderNotFound;
        }
        var now = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(
            _timeProvider.GetUtcNow(),
            "Africa/Cairo");

        if (now >= workOrder.StartAtUtc)
        {
            return WorkOrderErrors.StateTransitionNotAllowed(
                workOrder.StartAtUtc);
        }

        var duration = workOrder.EndAtUtc.Subtract(workOrder.StartAtUtc).Duration();
        var endAt = command.NewStartAt + duration;

        var spotCheckResult =
        await _workOrderPolicy.CheckSpotAvailabilityResult(
            command.Spot,
            command.NewStartAt,
            endAt,
            workOrder.Id,
            ct);
        if (spotCheckResult.IsError)
        {
            _logger.LogError("Spot: {spot} is not available",workOrder.Spot.ToString());
            return spotCheckResult.Errors!;
        }

        if(await _workOrderPolicy.IsLaborOccupied(
                workOrder.LaborId,
                workOrder.Id,
                command.NewStartAt,
                endAt))
        {
            _logger.LogError("Labor with ID '{laborId}' is already occupied during the requested time.",workOrder.LaborId);

            return ApplicationErrors.LaborOccupied;
        }
        
        if(await _workOrderPolicy.IsVehicleAlreadyScheduled(workOrder.VehicleId, command.NewStartAt, endAt, workOrder.Id))
        {
            _logger.LogError("Vehicle with ID '{vehicleId}' already has an overlapping workOrder.",workOrder.VehicleId);
            return ApplicationErrors.VehicleSchedulingConflict;
        }
        if (_workOrderPolicy.IsOutsideOperatingHours(
                command.NewStartAt,
                endAt))
            {
                return ApplicationErrors.WorkOrderOutsideOperatingHour(
                    command.NewStartAt,
                    endAt);
            }
        var updateTimeingResult =workOrder.UpdateTime(command.NewStartAt,endAt);
        if (updateTimeingResult.IsError)
        {
            _logger.LogError("Falid to update time {error}.",updateTimeingResult.TopError.Description);
            return updateTimeingResult.Errors!;
        }
        var updateSpotResult = workOrder.UpdateSpot(command.Spot);

        if (updateSpotResult.IsError)
        {
            _logger.LogError("Falid to update Spot {error}.",updateSpotResult.TopError.Description);
            return updateSpotResult.Errors!;
        }
        workOrder.AddDomainEvent(new WorkOrderCollectionModified());
        await _context.SaveChangesAsync(ct);
        
        await _cache.RemoveByTagAsync("work-order",ct);
        return Result.Updated;
    }
}