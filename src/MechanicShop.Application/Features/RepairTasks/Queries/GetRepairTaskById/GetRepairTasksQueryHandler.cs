using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.RepairTasks.Dtos;
using MechanicShop.Application.Features.RepairTasks.Mppers;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.RepairTasks.Queries.GetRepairTaskById;

public sealed class GetRepairTaskByIdQueryHandler(IAppDbContext context,ILogger<GetRepairTaskByIdQueryHandler> logger) : IRequestHandler<GetRepairTaskByIdQuery, Result<RepairTaskDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<GetRepairTaskByIdQueryHandler> _logger = logger;

    public async Task<Result<RepairTaskDto>> Handle(GetRepairTaskByIdQuery query, CancellationToken ct)
    {
        var repairTask = await _context.RepairTasks.AsNoTracking().Include(r => r.Parts)
        .FirstOrDefaultAsync(r => r.Id == query.RepairTaskId,ct);
        if(repairTask is null)
        {
            _logger.LogInformation("Repair Task with id '{taskName}' not found",query.RepairTaskId);
            return ApplicationErrors.RepairTaskNotFound;
        }

        return repairTask.ToDto();
    }
}