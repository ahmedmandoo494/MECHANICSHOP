using Asp.Versioning;
using MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask;
using MechanicShop.Application.Features.RepairTasks.Dtos;
using MechanicShop.Application.Features.RepairTasks.Queries.GetRepairTaskById;
using MechanicShop.Application.Features.RepairTasks.Queries.GetRepairTasks;
using MechanicShop.Contracts.Requests.RepairTasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

using MechanicShop.Domain.RepairTasks.Enums;
using MechanicShop.Application.Features.RepairTasks.Commands.UpdateRepairTask;
using MechanicShop.Application.Features.RepairTasks.Commands.RemoveRepairTask;
namespace MechanicShop.Api.Controllers;

[Route("api/v{version:ApiVersion}/repair-tasks")]
[ApiVersion("1.0")]
[Authorize]
public sealed class RepairTaskController(ISender sender) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(List<RepairTaskDto>),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves all repair tasks.")]
    [EndpointDescription("Returns a list of all repair tasks available in the system.")]
    [EndpointName("GetRepairTasks")]
    [MapToApiVersion("1.0")]
    [OutputCache(Duration =60)]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await sender.Send(new GetRepairTasksQuery(),ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("{repairTaskId:guid}",Name = nameof(GetById))]
    [ProducesResponseType(typeof(RepairTaskDto),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a repair task by id.")]
    [EndpointDescription("Returns detailed information for a specified repair task if it exist.")]
    [EndpointName("GetRepairTaskById")]
    [MapToApiVersion("1.0")]
    [OutputCache(Duration =60)]
    public async Task<IActionResult> GetById(Guid repairTaskId,CancellationToken ct)
    {
        var result = await sender.Send(new GetRepairTaskByIdQuery(repairTaskId),ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpPost]
    [ProducesResponseType(typeof(RepairTaskDto),StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Create a new repair task.")]
    [EndpointDescription("Creates a repair task and optionally incudes parts.")]
    [EndpointName("CreateRepairTask")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Create([FromBody] CreateRepairTaskRequest request,CancellationToken ct)
    {
        var parts = request.Parts.ConvertAll(p => new CreatePartsCommand(p.Name,p.Cost,p.Quantity)); 
        var result = await sender.Send(new CreateRepairTaskCommand(
            request.Name,
            request.LaborCost,
            request.RepairDurationInMins is not null ? (RepairDurationInMinute)request.RepairDurationInMins:null,
            parts)
            ,ct);
        return result.Match(
            response => CreatedAtRoute("GetById",new{version ="1.0",repairTaskId= response.RepairTaskId},response),
            Problem
        );
    }


    [HttpPut("{repairTaskId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Update an existing repair task.")]
    [EndpointDescription("Update a repair task and its associated parts.")]
    [EndpointName("UpdateRepairTask")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Update(Guid repairTaskId,[FromBody] UpdateRepairTaskRequest request,CancellationToken ct)
    {
        var parts = request.Parts.ConvertAll(p => new UpdatePartsCommand(p.PartId,p.Name,p.Cost,p.Quantity)); 
        var result = await sender.Send(new UpdateRepairTaskCommand(
            repairTaskId,
            request.Name,
            request.LaborCost,
            (RepairDurationInMinute)request.RepairDurationInMins,
            parts)
            ,ct);
        return result.Match(
            _ =>NoContent(),
            Problem
        );
    }

    [HttpDelete("{repairTaskId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Remove a repair task.")]
    [EndpointDescription("Remove the specified repair task from system.")]
    [EndpointName("RemoveRepairTask")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Delete(Guid repairTaskId,CancellationToken ct)
    {
        
        var result = await sender.Send(new RemoveRepairTaskCommand(
            repairTaskId)
            ,ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }
}
