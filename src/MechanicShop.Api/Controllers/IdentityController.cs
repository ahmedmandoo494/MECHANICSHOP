using System.Security.Claims;
using Asp.Versioning;
using MechanicShop.Application.Features.Identity;
using MechanicShop.Application.Features.Identity.Dtos;
using MechanicShop.Application.Features.Identity.Queries.GenerateToken;
using MechanicShop.Application.Features.Identity.Queries.GetUserInfo;
using MechanicShop.Application.Features.Identity.Queries.RefreshTokens;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace MechanicShop.Api.Controllers;

[Route("identity")]
[ApiVersionNeutral]
public sealed class IdentityController(ISender sender) : ApiController
{
    [HttpPost("token/generate")]
    [ProducesResponseType(typeof(TokenResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Generate access and refresh token for a valid user.")]
    [EndpointDescription("Authentictes a user using provided credentails and returns a JWT token.")]
    [EndpointName("GenenrateToken")]
    public async Task<IActionResult> GenerateToken([FromBody]GenerateTokenQuery request, CancellationToken ct)
    {
        var result = await sender.Send(request,ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }


    [HttpPost("token/refresh-token")]
    [ProducesResponseType(typeof(TokenResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Refreshe access token using a valid refresh token.")]
    [EndpointDescription("Exchanges an expired access token and a valid refresh token for a new token pair.")]
    [EndpointName("RefreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody]RefreshTokenQuery request, CancellationToken ct)
    {
        var result = await sender.Send(request,ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("current-user/claims")]
    [Authorize]
    [ProducesResponseType(typeof(AppUserDto),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Gets the current authenticated user's info.")]
    [EndpointDescription("Returns user information for the currently authentication user basd on the access token.")]
    [EndpointName("GetCurrentUserClaims")]
    public async Task<IActionResult> GetCurrentUserClaims(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await sender.Send(new GetUserByIdQuery(userId),ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }
}