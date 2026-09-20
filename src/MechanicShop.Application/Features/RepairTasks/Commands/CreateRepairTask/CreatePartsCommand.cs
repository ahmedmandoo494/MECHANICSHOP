using System.IO.Pipes;
using MechanicShop.Application.Features.RepairTasks.Dtos;
using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask;
public sealed record CreatePartsCommand(string Name,decimal Cost , int Quantity):IRequest<Result<Success>>;

