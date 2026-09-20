using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders.Enums;
using MediatR;

namespace MechanicShop.Application.Features.WorkOrders.Commands.RelocateWorkOrder;

public sealed record RelocateWorkOrderCommand(Guid WorkOrderId ,DateTimeOffset NewStartAt, Spots Spot):IRequest<Result<Updated>>;
