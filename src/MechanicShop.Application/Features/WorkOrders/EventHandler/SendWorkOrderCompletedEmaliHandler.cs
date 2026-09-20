using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.WorkOrders;
using MechanicShop.Domain.WorkOrders.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.WorkOrders.EventHandler;

public sealed class SendWorkOrderCompletedEmaliHandler(
    IAppDbContext context,
    ILogger<SendWorkOrderCompletedEmaliHandler> logger
    ,INotificationService notificationService)
    : INotificationHandler<WorkOrderCompleted>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<SendWorkOrderCompletedEmaliHandler> _logger = logger;
    private readonly INotificationService _notificationService = notificationService;

    public async Task Handle(WorkOrderCompleted notification, CancellationToken ct)
    {
        var workOrder = await _context.WorkOrders.Include(w => w.Vehicle).ThenInclude(v => v!.Customer)
        .AsNoTracking().FirstOrDefaultAsync(w => w.Id == notification.WorkOrderId,ct);

        if(workOrder is null)
        {
            _logger.LogError("WorkOrder with id {workorderid} does not exist.",notification.WorkOrderId);
            return;
        }

        await _notificationService.SendEmailAsync(workOrder.Vehicle?.Customer?.Email!,ct); 
        await _notificationService.SendSmsAsync(workOrder.Vehicle?.Customer?.PhoneNumber!,ct); 

    }
}
