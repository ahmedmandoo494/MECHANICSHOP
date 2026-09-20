using MechanicShop.Contracts.Common;

namespace MechanicShop.Contracts.Requests.WorkOrders;

public record UpdateWorkOrderStateRequest
{
    public WorkOrderStatus State{get;set;}
}


