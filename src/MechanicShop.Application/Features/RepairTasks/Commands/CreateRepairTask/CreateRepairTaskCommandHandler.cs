using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
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

namespace MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask;


public sealed class CreateRepairTaskCommandHandler(IAppDbContext context,ILogger<CreateRepairTaskCommandHandler> logger,HybridCache cache) : IRequestHandler<CreateRepairTaskCommand, Result<RepairTaskDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<CreateRepairTaskCommandHandler> _logger = logger;
    private readonly HybridCache _cache = cache;

    public async Task<Result<RepairTaskDto>> Handle(CreateRepairTaskCommand command, CancellationToken ct)
    {
        var nameExists = await _context.RepairTasks
        .AnyAsync(r => EF.Functions.Like(r.Name , command.Name),ct);
        if (nameExists)
        {
            _logger.LogInformation("Duplicate Repair Task name '{taskName}'",command.Name);
            return RepairTaskError.DuplicateName;
        }
        List<Part> parts = [] ;

        foreach(var part in command.Parts)
        {
            var innerPart = Part.Create(
                Guid.NewGuid(),
                part.Name,
                part.Cost,
                part.Quantity
            );
            if (innerPart.IsError)
            {
                return innerPart.Errors!;
            }
            parts.Add(innerPart.Value);
        }

        var createRepairTaskResult = RepairTask.Create(
            Guid.NewGuid(),
            command.Name,
            command.LaborCost,
            command.RepairDurationInMinute!.Value,
            parts
        );
        if (createRepairTaskResult.IsError)
        {
            return createRepairTaskResult.Errors!;
        }
        var repairTask = createRepairTaskResult.Value;
        _context.RepairTasks.Add(repairTask);
        await _context.SaveChangesAsync(ct);
        await _cache.RemoveAsync("repair-tasks",ct);

        return repairTask.ToDto();
    }
}