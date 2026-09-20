using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MechanicShop.Application.Features.WorkOrders.Dtos;
using MechanicShop.Application.Features.WorkOrders.Queries.GetWorkOrders;
using MechanicShop.Contracts.Requests.WorkOrders;
using Azure;
using MechanicShop.Domain.WorkOrders.Enums;
using MechanicShop.Application.Features.WorkOrders.Queries.GetWorkOrderByIdQuery;
using MechanicShop.Application.Features.WorkOrders.Commands.CreateWorkOrder;
using MechanicShop.Application.Features.WorkOrders.Commands.RelocateWorkOrder;
using MechanicShop.Application.Features.WorkOrders.Commands.AssingLabor;
using MechanicShop.Application.Features.WorkOrders.Commands.UpdateWorkOrderState;
using MechanicShop.Application.Features.RepairTasks.Commands.UpdateRepairTask;
using MechanicShop.Application.Features.WorkOrders.Commands.UpdateOrderRepairTask;
using Microsoft.EntityFrameworkCore;
using MechanicShop.Application.Features.WorkOrders.Commands.DeleteWorkOrder;
using MechanicShop.Application.Features.Scheduling.Dtos;
using MechanicShop.Application.Features.Scheduling.Queries;
namespace MechanicShop.Api.Controllers;

[Route("api/v{version:ApiVersion}/workorders")]
[ApiVersion("1.0")]
[Authorize]
public sealed class WorkOrderController(ISender sender) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(List<WorkOrderDto>),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a paginated list of work orders.")]
    [EndpointDescription("Supports filtering by date range, status, vehicle, labor, spot, and searching by trem, pagination and sorting are supported.")]
    [EndpointName("GetWorkOrders")]
    [MapToApiVersion("1.0")]

    public async Task<IActionResult> Get([FromQuery] WorkOrderFilterRequest filter,[FromQuery] PageRequest pageRequest ,CancellationToken ct)
    {
        var result = await sender.Send(new GetWorkOrdersQuery(
            pageRequest.Page,
            pageRequest.PageSize,
            filter.SearchTerm,
            filter.SortColumn,
            filter.SortDirection,
            filter.State is not null? (WorkOrderStatus)(int) filter.State:null,
            filter.VehicleId,
            filter.LaborId,
            filter.StartDateFrom,
            filter.StartDateTo,
            filter.EndDateFrom,
            filter.EndDateTo,
            filter.Spot is not null ? (Spots)(int)filter.Spot:null

        ),ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("{WorkOrderId:guid}",Name ="GetWorkOrderById")]
    [ProducesResponseType(typeof(WorkOrderDto),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a work order by ID.")]
    [EndpointDescription("Returns detailed inforamtion about the specified work order if it exists.")]
    [EndpointName("GetWorkOrderById")]
    [MapToApiVersion("1.0")]

    public async Task<IActionResult> GetById(Guid WorkOrderId ,CancellationToken ct)
    {
        var result = await sender.Send(new GetWorkOrderByIdQuery(WorkOrderId),ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpPost]
    [Authorize(Policy = "ManagerOnly")]
    [ProducesResponseType(typeof(WorkOrderDto),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Create a new work order.")]
    [EndpointDescription("Create a new work order for vehicle, specifying labor, tasks, and other required information.")]
    [EndpointName("CreateWorkOrder")]
    [MapToApiVersion("1.0")]

    public async Task<IActionResult> Create([FromBody]  CreateWorkOrderRequest request,CancellationToken ct)
    {
        var result = await sender.Send(new CreateWorkOrderCommand(
            (Spots)(int)request.Spot,
            request.VehicleId,
            request.StartAt,
            request.RepairTaskIds,
            request.LaborId
        ),ct);
        return result.Match(
            response => CreatedAtRoute("GetWorkOrderById",new{version ="1.0",WorkOrderId = response.WorkOrderId},response),
            Problem
        );
    }

    [HttpPut("{workOrderId:guid}/relocation")]
    [Authorize(Policy = "ManagerOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Relocates a work order to a new spot and time.")]
    [EndpointDescription("Update the scheduled time and assigned by for a work order. Only users with the manager role can perform this action.")]
    [EndpointName("RelocateWorkOrder")]
    [MapToApiVersion("1.0")]

    public async Task<IActionResult> Relocate(Guid workOrderId, [FromBody] RelocateWorkOrderRequest request,CancellationToken ct)
    {
        var result = await sender.Send(new RelocateWorkOrderCommand(
            workOrderId,
            request.NewStartAtUtc,
            (Spots)(int)request.NewSpot),
            ct);

        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpPut("{workOrderId:guid}/labor")]
    [Authorize(Policy = "ManagerOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Assigns a labor to a work order.")]
    [EndpointDescription("Associates a labor definition with a specific work order. Only managers can perform this operation.")]
    [EndpointName("AssignLaborToWorkOrder")]
    [MapToApiVersion("1.0")]

    public async Task<IActionResult> Labor(Guid workOrderId, [FromBody] AssignLaborRequest request,CancellationToken ct)
    {
        var result = await sender.Send(new AssignLaborCommand(Guid.Parse(request.LaberId),workOrderId),ct);

        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpPut("{workOrderId:guid}/state")]
    [Authorize(Policy = "ManagerOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Changes the state of a work order.")]
    [EndpointDescription("updates the current state fo the specified work order. Only managers and Labor associated with this work order can perform this operation.")]
    [EndpointName("UpdateWorkOrderState")]
    [MapToApiVersion("1.0")]

    public async Task<IActionResult> UpdateState(Guid workOrderId, [FromBody] UpdateWorkOrderStateRequest request,CancellationToken ct)
    {
        var result = await sender.Send(new UpdateOrderStateCommand(
            workOrderId,
            (WorkOrderStatus)(int)request.State),
            ct);

        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpPut("{workOrderId:guid}/repair-task")]
    [Authorize(Policy = "ManagerOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Modify and And add repair tasks to the work order.")]
    [EndpointDescription("updates the current repair tasks fo the specified work order. Only managers can perform this operation.")]
    [EndpointName("ModifyWorkOrderRepairTask")]
    [MapToApiVersion("1.0")]

    public async Task<IActionResult> UpdateRepairTask(Guid workOrderId, [FromBody] ModifyRepairTaskRequest request,CancellationToken ct)
    {
        var result = await sender.Send(new UpdateWorkOrderRepairTaskCommand(
            request.RepairTaskIds,
            workOrderId),
            ct);

        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpDelete("{workOrderId:guid}")]
    [Authorize(Policy = "ManagerOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Deletes a work order.")]
    [EndpointDescription("Delete the specified work order permanently. Only managers can perform this operation.")]
    [EndpointName("DeleteWorkOrder")]
    [MapToApiVersion("1.0")]

    public async Task<IActionResult> Delete(Guid workOrderId,CancellationToken ct)
    {
        var result = await sender.Send(new DeleteWorkOrderCommand(workOrderId),ct);

        return result.Match(
            _ => NoContent(),
            Problem
        );
    }


    [HttpGet("schedule/{date}")]
    [Authorize]
    [ProducesResponseType(typeof(ScheduleDto),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a work order by ID.")]
    [EndpointDescription("Returns detailed inforamtion about the specified work order if it exists.")]
    [EndpointName("GetSchedule")]
    [MapToApiVersion("1.0")]

    public async Task<IActionResult> GetSchedule(
        DateOnly? date,
        [FromQuery] Guid? LaborId,
        [FromHeader(Name = "X-TimeZone")] string? tz,
        CancellationToken ct)
    {

        if (string.IsNullOrWhiteSpace(tz))
        {
            return Problem(
                detail:"Missing time zone in 'X-TimeZone' header",
                statusCode: StatusCodes.Status400BadRequest,
                title:"Time Zone Required"
            );
        }
        TimeZoneInfo timeZone;
        try
        {
            timeZone = TimeZoneInfo.FindSystemTimeZoneById(tz);
        }
        catch
        {
            return Problem(
                detail:$"Invalid or unknown time zone in: '{tz}'.",
                statusCode: StatusCodes.Status400BadRequest,
                title:"Invlid Time Zone"
            );
        }
        var scheduleDate =date ?? DateOnly.FromDateTime(DateTime.UtcNow);



        var result = await sender.Send(new GetDailyScheduleQuery(timeZone,scheduleDate,LaborId),ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }
}