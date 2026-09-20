namespace MechanicShop.Application.Features.RepairTasks.Dtos;

public sealed class PartDto
{
    public Guid PartId { get; set; }
    public string? Name { get; set; }
    public decimal Cost{get; set;}
    public int Quantity{get; set;}
}