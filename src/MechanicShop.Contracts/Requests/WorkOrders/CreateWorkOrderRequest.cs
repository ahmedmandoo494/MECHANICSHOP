using System.ComponentModel.DataAnnotations;
using MechanicShop.Contracts.Common;

namespace MechanicShop.Contracts.Requests.WorkOrders;

public record CreateWorkOrderRequest
{
    [Required(ErrorMessage = "Spot is required.")]
    [Range(0,3,ErrorMessage ="Invalid range [0, 1, 2, 3]")]
    public Spots Spot{get;set;}

    [Required(ErrorMessage = "Vehicle is required")]
    public Guid VehicleId{get;set;}

    [MinLength(1,ErrorMessage = "At least one repair task is required.")]
    public List<Guid> RepairTaskIds{get;set;}=[];

    [Required(ErrorMessage = "Labor is required.")]
    public Guid LaborId{get;set;}

    [Required(ErrorMessage = "StartAt is required.")]
    public DateTimeOffset StartAt { get; set; }
}