using Asp.Versioning;
using MechanicShop.Application.Features.Labors.Dtos;
using MechanicShop.Application.Features.Labors.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;


namespace MechanicShop.Api.Controllers;

[Route("api/v{version:ApiVersion}/labors")]
[ApiVersion("1.0")]
[Authorize]
public sealed class LaborsController(ISender sender) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(List<LaborDto>),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves the list of Labor definitins.")]
    [EndpointDescription("Returns all labors associated with the system, accessible only to users the manager role.")]
    [EndpointName("GetLabors")]
    [MapToApiVersion("1.0")]
    [OutputCache(Duration = 60)]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await sender.Send(new GetLaborQuery(),ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

}