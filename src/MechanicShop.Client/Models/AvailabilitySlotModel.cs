using MechanicShop.Contracts.Common;

namespace MechanicShop.Client.Models;
public class AvailabilitySlotModel
{
    public Guid? WorkOrderId { get; set; }
    public Spots Spot { get; set; }
    public DateTimeOffset StartAt { get; set; }
    public DateTimeOffset EndAt { get; set; }
    public string? Vehicle { get; set; }
    public RepairTaskModel[] RepairTasks { get; set; } = [];
    public LaborModel? Labor { get; set; }
    public bool IsOccupied { get; set; }
    public bool IsAvaliable { get; set; }
    public bool WorkOrderLocked { get; set; }
    public WorkOrderStatus? State { get; set; }
}


// public class ScheduleModel
// {
//     public DateOnly OnDate { get; set; }
//     public bool EndOfDay { get; set; }
//     public List<SpotModel> Spots { get; set; } = [];
// }


// public class SpotModel
// {
//     public Spots Spot { get; set; }
//     public List<AvailabilitySlotModel> Slots { get; set; } = [];
// }


