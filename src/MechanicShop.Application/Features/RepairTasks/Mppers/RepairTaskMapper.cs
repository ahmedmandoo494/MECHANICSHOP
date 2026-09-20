using MechanicShop.Application.Features.RepairTasks.Dtos;
using MechanicShop.Domain.RepairTasks;

namespace MechanicShop.Application.Features.RepairTasks.Mppers;



public static class RepairTaskMapper
{
    public static RepairTaskDto ToDto(this RepairTask entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new RepairTaskDto
        {
            RepairTaskId = entity.Id,
            Name = entity.Name,
            LaborCost = entity.LaborCost,
            TotalCost = entity.TotalCost,
            RepairDurationInMinute = entity.RepairDurationInMinute,
            Parts = entity.Parts.ToDtos()
        };
    }

    public static List<RepairTaskDto> ToDtos(this IEnumerable<RepairTask> entities)
    {
        return [.. entities.Select(e => e.ToDto())];
    }
}
