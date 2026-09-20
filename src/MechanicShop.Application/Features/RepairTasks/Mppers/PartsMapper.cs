using MechanicShop.Application.Features.RepairTasks.Dtos;
using MechanicShop.Domain.RepairTasks.Parts;

namespace MechanicShop.Application.Features.RepairTasks.Mppers;

public static class PartsMapper
{
    public static PartDto ToDto(this Part entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new PartDto
        {
            PartId = entity.Id,
            Name = entity.Name!,
            Cost = entity.Cost,
            Quantity = entity.Quantity
        };
    }

    public static List<PartDto> ToDtos(this IEnumerable<Part> entities)
    {
        return [.. entities.Select(e => e.ToDto())];
    }
}