using MechanicShop.Domain.Common;

namespace MechanicShop.Domain.WorkOrders.Events;

public sealed class WorkOrderCompleted : DomainEvent
{
    public Guid WorkOrderId { get;}
    public WorkOrderCompleted(Guid workOrderId)
    {
        WorkOrderId = workOrderId;
    }
}
