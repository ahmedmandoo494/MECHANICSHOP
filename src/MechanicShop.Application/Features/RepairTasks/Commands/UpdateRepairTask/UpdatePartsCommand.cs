using System.IO.Pipes;
using MechanicShop.Application.Features.RepairTasks.Dtos;
using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.RepairTasks.Commands.UpdateRepairTask;
public sealed record UpdatePartsCommand(Guid? PartId,string? Name,decimal Cost , int Quantity);

