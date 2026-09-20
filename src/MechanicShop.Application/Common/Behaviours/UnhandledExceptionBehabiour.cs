using MediatR;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Common.Behaviours;


public class UnhandledExceptionBehabiour<Trequset, TResponse>(ILogger<Trequset> logger)

: IPipelineBehavior<Trequset, TResponse> where Trequset : notnull
{
    private readonly ILogger<Trequset> _logger = logger;

    public async Task<TResponse> Handle(Trequset request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        try
        {
            return await next(ct);
        }
        catch (Exception ex)
        {
            var requestName = typeof(Trequset).Name;
            _logger.LogError(ex,"Request: Unhandled Exception for Request {name} {@request}",requestName,request);
            throw;
        }
    }
}
