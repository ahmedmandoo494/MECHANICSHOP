using Asp.Versioning;
using MechanicShop.Application.Features.Dashboard.Dtos;
using MechanicShop.Application.Features.Dashboard.Queries.GetWorkOrderStats;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace MechanicShop.Api.Controllers;

[Route("api/v{version:ApiVersion}/dashboard")]
[ApiVersion("1.0")]
[Authorize]
public sealed class DashboardController(ISender sender) : ApiController
{
    [HttpGet("stats")]
    [ProducesResponseType(typeof(TodayWorkOrderStatsDto),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetToDayStats([FromBody] DateOnly? date,CancellationToken ct)
    {
        var statsDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var result = await sender.Send(new GetWorkOrderStatsQuery(statsDate),ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }
}
