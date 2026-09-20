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

namespace MechanicShop.Application.Features.WorkOrders.Commands.UpdateWorkOrderState;

public sealed class UpdateOrderStateCommandHandler(
    IAppDbContext context,
    ILogger<UpdateOrderStateCommandHandler> logger,
    HybridCache cache,
    TimeProvider timeProvider) : IRequestHandler<UpdateOrderStateCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<UpdateOrderStateCommandHandler> _logger = logger;
    private readonly HybridCache _cache = cache;
    private readonly TimeProvider _timeProvider = timeProvider;


    public async Task<Result<Updated>> Handle(UpdateOrderStateCommand command, CancellationToken ct)
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

        if (workOrder.StartAtUtc > now)
        {
            _logger.LogError("State transition for WorkOrder ID '{WorkOrderId}' is not allowd before the work order's scheduled start time.",command.WorkOrderId);
            return WorkOrderErrors.StateTransitionNotAllowed(workOrder.StartAtUtc);
        }

        var updateOrderStateResult = workOrder.UpdateStatus(command.State);

        if (updateOrderStateResult.IsError)
        {
            _logger.LogError("Faild to Update Status: {errors}",updateOrderStateResult.TopError.Description);
            return updateOrderStateResult.Errors!;

        }
        if(command.State == WorkOrderStatus.Completed)
        {
            
            workOrder.AddDomainEvent(new WorkOrderCompleted(command.WorkOrderId));
        }

        await _context.SaveChangesAsync(ct);
        workOrder.AddDomainEvent(new WorkOrderCollectionModified());
        await _cache.RemoveByTagAsync("work-order",ct);
        return Result.Updated;
    }
}