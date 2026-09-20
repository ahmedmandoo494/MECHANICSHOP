using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Application.Features.Labors.Dtos;
using MechanicShop.Application.Features.RepairTasks.Dtos;


using MechanicShop.Domain.WorkOrders.Enums;

namespace MechanicShop.Application.Features.WorkOrders.Dtos;

public sealed class WorkOrderDto
{
    public Guid WorkOrderId { get; set; }
    public Guid VehicleId { get; set;}
    public Guid? InvoiceId { get; set;}
    public Guid LaborId { get;  set;}
    public Spots Spot { get;  set; }
    public VehicleDto? Vehicle { get;  set; }
    public DateTimeOffset StartAtUtc { get;  set; }
    public DateTimeOffset EndAtUtc { get;  set; }
    public LaborDto? Labor{get;  set;}
    public List<RepairTaskDto> RepairTasks{get;set;}=[]; 
    public int TotalDurationInMins { get; set; }
    public WorkOrderStatus Status { get;  set; }
    public decimal? TotalPartCost{ get;  set; }
    public decimal? TotalLaborCost{ get;  set; }
    public decimal? TotalCost { get;  set; }
    public DateTimeOffset CreatedAt { get; set; }

}