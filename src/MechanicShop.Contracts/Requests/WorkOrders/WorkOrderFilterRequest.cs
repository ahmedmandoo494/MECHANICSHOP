using MechanicShop.Contracts.Common;

namespace MechanicShop.Contracts.Requests.WorkOrders;

public record WorkOrderFilterRequest
{
    public string? SearchTerm{get;set;}
    public string SortColumn{get;set;} = "createdAt";
    public string SortDirection{get;set;} = "desc";
    public WorkOrderStatus? State{get;set;} = null;
    public Guid? VehicleId{get;set;} = null;
    public Guid? LaborId{get;set;} = null;
    public DateTime? StartDateFrom{get;set;} = null;
    public DateTime? StartDateTo{get;set;} = null;
    public DateTime? EndDateFrom{get;set;} = null;
    public DateTime? EndDateTo{get;set;} = null;
    public Spots? Spot{get;set;} = null;
}
