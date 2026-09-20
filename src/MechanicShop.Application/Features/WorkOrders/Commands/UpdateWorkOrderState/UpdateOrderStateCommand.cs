using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders.Enums;
using MediatR;

namespace MechanicShop.Application.Features.WorkOrders.Commands.UpdateWorkOrderState;

public sealed record UpdateOrderStateCommand(Guid WorkOrderId, WorkOrderStatus State):IRequest<Result<Updated>>;
