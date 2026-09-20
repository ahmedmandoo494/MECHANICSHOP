using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.WorkOrders.Commands.AssingLabor;

public sealed class AssignLaborCommandHandler(
    IAppDbContext context,
    ILogger<AssignLaborCommandHandler> logger,
    HybridCache cache,
    IWorkOrderPolicy workOrderPolicy) : IRequestHandler<AssignLaborCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<AssignLaborCommandHandler> _logger = logger;
    private readonly HybridCache _cache = cache;
    private readonly IWorkOrderPolicy _workOrderPolicy = workOrderPolicy;

    public async Task<Result<Updated>> Handle(AssignLaborCommand command, CancellationToken ct)
    {
        var workOrder = await _context.WorkOrders.FirstOrDefaultAsync(w => w.Id == command.WorkOrderId,ct);
        if(workOrder is null)
        {
            return ApplicationErrors.WorkOrderNotFound;
        }

        var labor = await _context.Employees.FindAsync([command.LaborId],ct);
        if(labor is null)
        {
            return ApplicationErrors.LaborNotFound;
        }

        if (await _workOrderPolicy.IsLaborOccupied(command.LaborId,command.WorkOrderId,workOrder.StartAtUtc,workOrder.EndAtUtc))
        {
            return ApplicationErrors.LaborOccupied;
        }

        var updateLaborResult = workOrder.UpdateLabor(command.LaborId);
        if (updateLaborResult.IsError)
        {
            return updateLaborResult.Errors!;
        }

        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("work-order", ct);
        return Result.Updated;

    }
}