namespace MechanicShop.Contracts.Requests.WorkOrders;

public record ModifyRepairTaskRequest
{
    public Guid[] RepairTaskIds {get;set;}=[];
}


