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

namespace MechanicShop.Application.Features.RepairTasks.Commands.UpdateRepairTask;


public sealed class UpdateRepairTaskCommandHandler(IAppDbContext context,ILogger<UpdateRepairTaskCommandHandler> logger,HybridCache cache) : IRequestHandler<UpdateRepairTaskCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<UpdateRepairTaskCommandHandler> _logger = logger;
    private readonly HybridCache _cache = cache;

    public async Task<Result<Updated>> Handle(UpdateRepairTaskCommand command, CancellationToken ct)
    {
        var repairTask = await _context.RepairTasks.Include(r => r.Parts)
        .FirstOrDefaultAsync(r => r.Id == command.TaskId,ct);
        if (repairTask is null)
        {
            _logger.LogInformation("Repair Task with id '{taskName}' not found for update",command.TaskId);
            return ApplicationErrors.RepairTaskNotFound;
        }
        var validatedParts = new List<Part>();

        foreach (var p in command.Parts)
        {
            var partId = p.PartId ?? Guid.NewGuid();
            var partResult = Part.Create(partId,p.Name,p.Cost,p.Quantity);
            if (partResult.IsError)
            {
                return partResult.Errors!;
            }
            validatedParts.Add(partResult.Value);
        }
        var updateRepairTaskResult = repairTask.Update(command.Name,command.LaborCost,command.RepairDurationInMinute);
        if (updateRepairTaskResult.IsError)
        {
            return updateRepairTaskResult.Errors!;
        }

        var updatePartResult = repairTask.UpsertParts(validatedParts);
        if(updatePartResult.IsError)
        {
            return updatePartResult.Errors!;
        }

        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("repair-tasks",ct);
        return Result.Updated;
    }
}