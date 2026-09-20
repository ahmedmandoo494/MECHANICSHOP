using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.WorkOrders.Dtos;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using MechanicShop.Application.Features.WorkOrders.Mappers;
using System.Runtime.InteropServices;

namespace MechanicShop.Application.Features.WorkOrders.Commands.CreateWorkOrder;

public sealed class CreateWorkOrderCommandHandler(
    IAppDbContext context,
    IWorkOrderPolicy workOrderPolicy,
    ILogger<CreateWorkOrderCommandHandler> logger,
    HybridCache cache):IRequestHandler<CreateWorkOrderCommand,Result<WorkOrderDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly IWorkOrderPolicy _workOrderPolicy = workOrderPolicy;
    private readonly ILogger<CreateWorkOrderCommandHandler> _logger = logger;
    private readonly HybridCache _cache = cache;

    public async Task<Result<WorkOrderDto>> Handle(CreateWorkOrderCommand command, CancellationToken ct)
    {

        _logger.LogInformation(
    "CREATE WORK ORDER TIME -> StartAt: {StartAt}, Offset: {Offset}",
    command.StartAt,
    command.StartAt.Offset);
        var repairTasks = await _context.RepairTasks
        .Where(rt => command.RepairTaskIds.Contains(rt.Id)).ToListAsync(ct);

        if(repairTasks.Count != command.RepairTaskIds.Count)
        {
            var missingIds = command.RepairTaskIds.Except(repairTasks.Select(r => r.Id)).ToArray();
            _logger.LogInformation("Some repair tasks not found: {MissingIds}",missingIds);
            return ApplicationErrors.RepairTaskNotFound;
        }

        var totalEstimatedDuration = TimeSpan.FromMinutes(repairTasks.Sum(rt => (int)rt.RepairDurationInMinute));
        var endAt = command.StartAt.Add(totalEstimatedDuration);

        if (_workOrderPolicy.IsOutsideOperatingHours(command.StartAt, endAt))
        {
            _logger.LogInformation("The WorkOrder time {start} ? {end} is outeside of store operating Hours",command.StartAt,endAt);

            return ApplicationErrors.WorkOrderOutsideOperatingHour(command.StartAt,endAt);
        }
        var ValidateMinimumRequirementResult = _workOrderPolicy.ValidateMinimumRequirement(command.StartAt,endAt);
        if(ValidateMinimumRequirementResult.IsError)
        {
            _logger.LogInformation("Work Order duration is shorter than the configured minimun. ");

            return ValidateMinimumRequirementResult.Errors!;
        }
        var vehicle =await _context.Vehicles.Include(v=>v.Customer).FirstOrDefaultAsync(v => v.Id == command.VehicleId);

        if(vehicle is null)
        {
            _logger.LogInformation("Vehicle with id {vehicleId} not found",command.VehicleId);

            return ApplicationErrors.VehicleNotFound;
        }
        var labor =await _context.Employees.FindAsync([command.LaborId],ct);
        if(labor is null)
        {
            _logger.LogInformation("Labor with id {laborId} not found",command.LaborId);

            return ApplicationErrors.LaborNotFound;
        }
        var CheckSpotAvailability =await _workOrderPolicy.CheckSpotAvailabilityResult(
            command.Spot,
            command.StartAt,
            endAt,
            null,
            ct);
        if (CheckSpotAvailability.IsError)
        {
            _logger.LogInformation("Spot {spot} has already and overlapping work order",command.Spot);
            return CheckSpotAvailability.Errors!;
        }
        var hasVehicleConflect =await _context.WorkOrders.AnyAsync(
            w=>
            w.VehicleId == command.VehicleId &&
            w.StartAtUtc < endAt &&
            w.EndAtUtc > command.StartAt,
            ct
        );

        if (hasVehicleConflect)
        {
            _logger.LogInformation("vehicle with id {vehicleId} has already and overlapping work order",command.VehicleId);
            return ApplicationErrors.VehicleSchedulingConflict;
        }

        var hasLaborOccupied = await _context.WorkOrders
        .AnyAsync(w =>
        w.LaborId == command.LaborId &&
        w.StartAtUtc < endAt &&
        w.EndAtUtc > command.StartAt,
        ct);
        if (hasLaborOccupied)
        {
            _logger.LogInformation("Labor with id {LaborId} has already and overlapping work order",command.LaborId);
            return ApplicationErrors.LaborOccupied;
        }

        var createWorkOrderResult = WorkOrder.Create(
            Guid.NewGuid(),
            command.VehicleId,
            command.StartAt,
            endAt,
            command.LaborId,
            command.Spot,
            repairTasks
        );
        if (createWorkOrderResult.IsError)
        {
            _logger.LogInformation("Falied to create WorkOrder: {Error}",createWorkOrderResult.TopError.Description);
            return createWorkOrderResult.Errors!;
        }
        var workOrder = createWorkOrderResult.Value;
        _context.WorkOrders.Add(workOrder);

        await _context.SaveChangesAsync(ct);
        workOrder.Vehicle =vehicle;
        workOrder.Labor = labor;
        _logger.LogInformation("Work Order with id {workorderid} created successfuly",workOrder.Id);
        await _cache.RemoveByTagAsync("work-order",ct);
        return workOrder.ToDto();
    }
}