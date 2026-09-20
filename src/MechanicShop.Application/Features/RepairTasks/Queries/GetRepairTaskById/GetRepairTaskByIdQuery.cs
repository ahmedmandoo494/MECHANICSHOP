using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.RepairTasks.Dtos;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Features.RepairTasks.Queries.GetRepairTaskById;

public sealed record GetRepairTaskByIdQuery(Guid RepairTaskId)  : ICacheQuery<Result<RepairTaskDto>>
{
    public string Cachekey => $"repair-tasks";

    public string[] Tags => ["repair-tasks"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}