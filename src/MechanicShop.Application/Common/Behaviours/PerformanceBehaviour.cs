using System.Diagnostics;
using MechanicShop.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Common.Behaviours;

public class PerformanceBehaviour<TRequset, TResponse> : IPipelineBehavior<TRequset, TResponse> where TRequset:notnull
{
    private readonly Stopwatch _timer;
    private readonly ILogger<TRequset> _logger;
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public PerformanceBehaviour( ILogger<TRequset> logger, IUser user, IIdentityService identityService)
    {
        _timer = new Stopwatch();
        _logger = logger;
        _user = user;
        _identityService = identityService;
    }

    public async Task<TResponse> Handle(TRequset request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        _timer.Start();
        var response = await next(ct);
        _timer.Stop();
        var elapsedMilliseconds =_timer.ElapsedMilliseconds;

        if(elapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequset).Name;
            var userId = _user.Id ?? string.Empty;
            var userName = string.Empty;

            if (!string.IsNullOrEmpty(userId))
            {
                userName = await _identityService.GetUserNameAsync(userId);
            }
            _logger.LogWarning("Long Running Request: {name} ({elapsedMilliseconds} Milliseconds) {userId} {@userName} {@requset}"
            ,requestName,elapsedMilliseconds,userId,userName,request);
        }
        return response;
    }
}