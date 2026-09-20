using MechanicShop.Application.Features.Customers.Mapper;
using MechanicShop.Application.Features.Labors.Dtos;
using MechanicShop.Application.Features.RepairTasks.Mppers;
using MechanicShop.Application.Features.WorkOrders.Dtos;
using MechanicShop.Domain.WorkOrders;

namespace MechanicShop.Application.Features.WorkOrders.Mappers;

public static class WorkOrderMapper
{
    public static WorkOrderDto ToDto(this WorkOrder entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new WorkOrderDto
        {
            WorkOrderId = entity.Id,
            VehicleId = entity.VehicleId,
            LaborId = entity.LaborId,
            Spot = entity.Spot,
            StartAtUtc = entity.StartAtUtc,
            EndAtUtc = entity.EndAtUtc,
            Labor = entity.Labor is null ? null : new LaborDto
            {
                LaborId = entity.LaborId,
                Name = $"{entity.Labor.FirstName} {entity.Labor.LastName}"
            },

            RepairTasks = entity.RepairTasks.ToDtos(),

            Vehicle = entity.Vehicle is null ? null : entity.Vehicle.ToDto(),
            Status = entity.Status,
            TotalPartCost = entity.TotalPartCost,
            TotalLaborCost = entity.TotalLaborCost,
            TotalCost = entity.Total,
            TotalDurationInMins = entity.RepairTasks.Sum(rt => (int)rt.RepairDurationInMinute),
            InvoiceId = entity.Invoice?.Id,
            CreatedAt = entity.CreatedAtUtc
        };
    }


    public static List<WorkOrderDto> ToDtos(this IEnumerable<WorkOrder> entities)
    {
        return [.. entities.Select(e => e.ToDto())];
    }



    public static WorkOrderListItemDto ToListItemDto(this WorkOrder entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new WorkOrderListItemDto
        {
            WorkOrderId = entity.Id,
            Spots = entity.Spot,
            InvoiceId = entity.Invoice?.Id,
            StartAtUtc = entity.StartAtUtc,
            EndAtUtc = entity.EndAtUtc,
            Vehicle = entity.Vehicle!.ToDto(),
            Labor = entity.Labor is null ? null :
                $"{entity.Labor.FirstName} {entity.Labor.LastName}",
            State = entity.Status,
            RepairTasks = entity.RepairTasks.Select(rt => rt.Name).ToList()!
        };
    }
}