using FluentValidation;
using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Domain.WorkOrders;
using MechanicShop.Domain.WorkOrders.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.WorkOrders.Commands.UpdateOrderRepairTask;

public sealed class UpdateWorkOrderRepairTaskCommandHandler(
    IAppDbContext context,
    ILogger<UpdateWorkOrderRepairTaskCommand> logger,
    HybridCache cache,
    IWorkOrderPolicy workOrderPolicy,
    TimeProvider timeProvider) : IRequestHandler<UpdateWorkOrderRepairTaskCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<UpdateWorkOrderRepairTaskCommand> _logger = logger;
    private readonly HybridCache _cache = cache;
    private readonly IWorkOrderPolicy _workOrderPolicy = workOrderPolicy;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<Result<Updated>> Handle(UpdateWorkOrderRepairTaskCommand command, CancellationToken ct)
    {
        var workOrder = await _context.WorkOrders.FirstOrDefaultAsync(w => w.Id == command.WorkOrderId,ct);
        if(workOrder is null)
        {
            _logger.LogError("WorkOrder with Id '{workorderid} does not exist.'",command.WorkOrderId);
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
        if(command.RepairTaskIds.Length == 0)
        {
            _logger.LogError("Empty RepairTaskIds list submitted.");
            return RepairTaskError.AtLeastOneRepairTaskisRequired;
        }

        var repairTasks = await _context.RepairTasks.Where(r => command.RepairTaskIds.Contains(r.Id)).ToListAsync(ct);

        if(repairTasks.Count != command.RepairTaskIds.Length)
        {

            var missingIds = command.RepairTaskIds.Except(repairTasks.Select(r => r.Id)).ToArray();
            _logger.LogError("One or more RepairTasks not found. {ids}",string.Join(", ",missingIds));
            return ApplicationErrors.RepairTaskNotFound;
        }

      
        var totalDuration =
            TimeSpan.FromMinutes(
                repairTasks.Sum(r => (int)r.RepairDurationInMinute));

        var endAt = workOrder.StartAtUtc + totalDuration;

        if (_workOrderPolicy.IsOutsideOperatingHours(
            workOrder.StartAtUtc,
            endAt))
        {
            return ApplicationErrors.WorkOrderOutsideOperatingHour(
                workOrder.StartAtUtc,
                endAt);
        }

        var spotCheckResult =
            await _workOrderPolicy.CheckSpotAvailabilityResult(
                workOrder.Spot,
                workOrder.StartAtUtc,
                endAt,
                workOrder.Id,
                ct);

        if (spotCheckResult.IsError)
        {
            return spotCheckResult.Errors!;
        }

        if (await _workOrderPolicy.IsLaborOccupied(
            workOrder.LaborId,
            workOrder.Id,
            workOrder.StartAtUtc,
            endAt))
        {
            return ApplicationErrors.LaborOccupied;
        }
          var clearExistingResult = workOrder.ClearRepairTask();
        if (clearExistingResult.IsError)
        {
            return clearExistingResult.Errors!;
        }

        foreach (var task in repairTasks)
        {
            var addRepairTaskResult = workOrder.AddRepairTask(task);
            if (addRepairTaskResult.IsError)
            {
                return addRepairTaskResult.Errors!;
            }
        }

        workOrder.UpdateTime(workOrder.StartAtUtc,endAt);
        workOrder.AddDomainEvent(new WorkOrderCollectionModified());

        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("work-order",ct);
        return Result.Updated;
    }
}