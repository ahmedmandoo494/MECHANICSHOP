using System.Security.Claims;
using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Identity.Queries.RefreshTokens;

public sealed class RefreshTokenQueryHnalder(IAppDbContext context,IIdentityService identityService,ITokenProvider tokenProvider, ILogger<RefreshTokenQueryHnalder> logger) : IRequestHandler<RefreshTokenQuery, Result<TokenResponse>>
{
    private readonly IAppDbContext _context = context;
    private readonly IIdentityService _identityService = identityService;
    private readonly ITokenProvider _tokenProvider = tokenProvider;
    private readonly ILogger<RefreshTokenQueryHnalder> _logger = logger;

    public async Task<Result<TokenResponse>> Handle(RefreshTokenQuery query, CancellationToken ct)
    {
        var principal =_tokenProvider.GetPrincipalFromExpiredToken(query.ExpiredAccessToken);
        if(principal is null)
        {
            _logger.LogError("Expired access token is not valid");
            return ApplicationErrors.ExpiredAccessTokenInvalid;
        }

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if(userId is null)
        {
            _logger.LogError("Invalid userId claim");
            return ApplicationErrors.UserIdClaimInvalid;
        }

        var userByIdResult =await _identityService.GetUserByIdAsync(userId);
        if (userByIdResult.IsError)
        {
            _logger.LogError("Get user by id error occurred: {error}",userByIdResult.Errors);
            return userByIdResult.Errors!;
        }

        var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == query.RefreshToken,ct);
        if(refreshToken is null)
        {
            _logger.LogError("Refresh token has expired.");
            return ApplicationErrors.RefreshTokenExpired;
        }

        var generateTokenResult = await _tokenProvider.GenerateJwtTokenAsync(userByIdResult.Value,ct);
        if (generateTokenResult.IsError)
        {
            _logger.LogError("Generate token error occurred: {error}",generateTokenResult.Errors);
            return generateTokenResult.Errors!;
        }

        return generateTokenResult.Value;
    }
}