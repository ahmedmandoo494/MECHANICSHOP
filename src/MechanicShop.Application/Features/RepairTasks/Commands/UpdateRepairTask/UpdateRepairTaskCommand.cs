using MechanicShop.Application.Features.RepairTasks.Dtos;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.RepairTasks.Enums;
using MediatR;

namespace MechanicShop.Application.Features.RepairTasks.Commands.UpdateRepairTask;
public sealed record UpdateRepairTaskCommand(Guid TaskId,string Name,decimal LaborCost,RepairDurationInMinute RepairDurationInMinute,List<UpdatePartsCommand> Parts):IRequest<Result<Updated>>;
