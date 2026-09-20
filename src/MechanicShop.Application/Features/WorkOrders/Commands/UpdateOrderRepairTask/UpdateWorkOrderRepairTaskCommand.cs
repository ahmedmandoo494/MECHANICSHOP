using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders;
using MediatR;

namespace MechanicShop.Application.Features.WorkOrders.Commands.UpdateOrderRepairTask;

public sealed record UpdateWorkOrderRepairTaskCommand(Guid[] RepairTaskIds,Guid WorkOrderId):IRequest<Result<Updated>>;
