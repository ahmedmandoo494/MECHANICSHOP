using System.Net.WebSockets;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.RepairTasks.Dtos;
using MechanicShop.Application.Features.RepairTasks.Mppers;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Domain.RepairTasks.Parts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.RepairTasks.Commands.RemoveRepairTask;


public sealed class RemoveRepairTaskCommandHandler(IAppDbContext context,ILogger<RemoveRepairTaskCommandHandler> logger,HybridCache cache) : IRequestHandler<RemoveRepairTaskCommand, Result<Deleted>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<RemoveRepairTaskCommandHandler> _logger = logger;
    private readonly HybridCache _cache = cache;

    public async Task<Result<Deleted>> Handle(RemoveRepairTaskCommand command, CancellationToken ct)
    {
        var repairTask = await _context.RepairTasks
        .FindAsync([command.TaskId],ct);
        if (repairTask is null)
        {
            _logger.LogInformation("Repair Task with id '{taskName}' not found for Deletion",command.TaskId);
            return ApplicationErrors.RepairTaskNotFound;
        }
        var isInUse =await _context.WorkOrders.AsNoTracking()
        .SelectMany(w => w.RepairTasks)
        .AnyAsync(r => r.Id == command.TaskId,ct);
        if (isInUse)
        {
            _logger.LogInformation("Repair Task {repairTask} cannot be deleled - in use by work order",command.TaskId);
            return RepairTaskError.InUse;
        }
       _context.RepairTasks.Remove(repairTask);
        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("repair-tasks",ct);
        _logger.LogInformation("Repair Task {repairTask} deleled successfuly",command.TaskId);
        return Result.Deleted;
    }
}