using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Identity.Queries.GenerateToken;

public sealed class GenerateTokenQueryHandler(ILogger<GenerateTokenQueryHandler> logger,ITokenProvider tokenProvider,IIdentityService identityService):IRequestHandler<GenerateTokenQuery,Result<TokenResponse>>
{
    private readonly ILogger<GenerateTokenQueryHandler> _logger = logger;
    private readonly ITokenProvider _tokenProvider = tokenProvider;
    private readonly IIdentityService _identityService = identityService;

    public async Task<Result<TokenResponse>> Handle(GenerateTokenQuery query, CancellationToken ct)
    {

        var userResponse = await _identityService.AuthenticationAsync(query.Email,query.Password);

        if (userResponse.IsError)
        {
            return userResponse.Errors!;
        }
        var generateTokenResult =await _tokenProvider.GenerateJwtTokenAsync(userResponse.Value,ct);
        if (generateTokenResult.IsError)
        {
            _logger.LogInformation("Generate token erro occurred: {ErrorDescription}",generateTokenResult.Errors);
            return generateTokenResult.Errors!;
        }
        return generateTokenResult.Value;
    }
}