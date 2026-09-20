using MechanicShop.Domain.RepairTasks.Enums;

namespace MechanicShop.Application.Features.RepairTasks.Dtos;



public sealed class RepairTaskDto
{
    public Guid RepairTaskId { get; set; }
    public string? Name { get; set; }
    public decimal LaborCost { get; set; }
    public decimal TotalCost { get; set; }
    public RepairDurationInMinute RepairDurationInMinute { get; set; }
    public List<PartDto> Parts{get;set;} =[];

}

