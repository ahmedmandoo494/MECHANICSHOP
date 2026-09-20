using MechanicShop.Application.Features.Labors.Dtos;
using MechanicShop.Application.Features.RepairTasks.Dtos;
using MechanicShop.Domain.Vehicles;
using MechanicShop.Domain.WorkOrders.Enums;

namespace MechanicShop.Application.Features.Scheduling.Dtos;



public class AvailabilitySlotDto
{
    public Guid WorkOrderId { get; set; }
    public Spots Spot { get; set; }
    public DateTimeOffset StartAt { get; set; }
    public DateTimeOffset EndAt { get; set; }
    public string? Vehicle { get; set; }
    public LaborDto? Labor { get; set; }
    public bool IsOccupied { get; set; }
    public bool IsAvaliable { get; set; }
    public bool WorkOrderLocked { get; set; }
    public WorkOrderStatus State { get; set; }
    public RepairTaskDto[]? RepairTasks { get; set; }

}
public class SpotDto
{
    public Spots Spot { get; set; }
    public List<AvailabilitySlotDto> Slots { get; set; } =[];
}

public class ScheduleDto
{
    public DateOnly OnDate { get; set; }
    public bool EndOfDay { get; set; }
    public List<SpotDto> Spots { get; set; }=[];
}