using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Identity.Dtos;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Identity.Queries.GetUserInfo;

public sealed class GetUserByIdQueryHandler(ILogger<GetUserByIdQueryHandler> logger,IIdentityService identityService):IRequestHandler<GetUserByIdQuery,Result<AppUserDto>>
{
    private readonly ILogger<GetUserByIdQueryHandler> _logger = logger;

    private readonly IIdentityService _identityService = identityService;

    public async Task<Result<AppUserDto>> Handle(GetUserByIdQuery query, CancellationToken ct)
    {
        var userByIdResult =await _identityService.GetUserByIdAsync(query.userId!);
        if (userByIdResult.IsError)
        {
            _logger.LogInformation("User with id {userId} {error}",query.userId,userByIdResult.Errors);
            return userByIdResult.Errors!;
        }
        return userByIdResult.Value;
    }
}