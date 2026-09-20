using MechanicShop.Application.Features.Labors.Dtos;
using MechanicShop.Domain.Employees;

namespace MechanicShop.Application.Features.Labors.Mappers;

public static class LaborMapper
{
    public static LaborDto ToDto(this Employee entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var labor = new LaborDto
        {
            LaborId = entity.Id,
            Name = entity.FullName
        };
        return labor;
    }

    public static List<LaborDto> ToDtos(this IEnumerable<Employee> entities)
    {
        return [..entities.Select(e => e.ToDto())];
    }

}