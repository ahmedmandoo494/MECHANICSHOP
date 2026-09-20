using Asp.Versioning;
using MechanicShop.Contracts.Responses;
using MechanicShop.Infrastructure.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;


namespace MechanicShop.Api.Controllers;

[Route("api/settings")]
[ApiVersionNeutral]
public sealed class SettingsController(IOptions<AppSettings> options) : ApiController
{
    private readonly AppSettings _appSettings = options.Value;

    [HttpGet("operating-hours")]
    [ProducesResponseType(typeof(OperatingHoursResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Issues an invoice for a work order.")]
    [EndpointDescription("Create a new invoice for the specified work order and returns the created invoice resource.")]
    [EndpointName("GetOperatingHours")]


    public IActionResult GetOperatingHours(CancellationToken ct)
    {
        
        return Ok(new OperatingHoursResponse(_appSettings.OpeningTime,_appSettings.ClosingTime));
    }
}