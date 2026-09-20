using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.WorkOrders.Events;
using MediatR;

namespace MechanicShop.Application.Features.WorkOrders.EventHandler;

public sealed class WorkOrderModifiedEventHandler(IWorkOrderNotifier workOrderNotifier)
    : INotificationHandler<WorkOrderCollectionModified>
{
    private readonly IWorkOrderNotifier _workOrderNotifier = workOrderNotifier;
    public Task Handle(WorkOrderCollectionModified notification, CancellationToken ct)
    {

        return _workOrderNotifier.NotifyWorkOrdersChangedAsync(ct);
    }
}