using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Common.Behaviours;

public sealed class CachingBehavior<TRequest, Tresponse>(HybridCache cache,
ILogger<CachingBehavior<TRequest, Tresponse>> logger) : IPipelineBehavior<TRequest, Tresponse> where TRequest :notnull
{
    private readonly HybridCache _cache = cache;
    private readonly ILogger<CachingBehavior<TRequest, Tresponse>> _logger = logger;

    public async Task<Tresponse> Handle(TRequest request, RequestHandlerDelegate<Tresponse> next, CancellationToken cancellationToken)
    {

        if(request is not ICacheQuery<Tresponse> cacheQuery)
        {
            return await next(cancellationToken);
        }
        _logger.LogInformation("Checking cache for {requestName}",typeof(TRequest).Name);
        var cacheKey = cacheQuery.Cachekey;

        var result =await _cache.GetOrCreateAsync(
            key: cacheKey,
            factory: async (CancellationToken) =>
            {
                var innerResult =  await next(cancellationToken);
                if(innerResult is IResult r && r.IsSuccess)
                {
                    return innerResult;
                }
                return default;
            },
            options: new HybridCacheEntryOptions
            {
                Expiration = cacheQuery.Expiration
            },
            tags:cacheQuery.Tags,
            cancellationToken:cancellationToken
        );
        return result!;
    }
}