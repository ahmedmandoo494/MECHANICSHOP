using MediatR;

namespace MechanicShop.Application.Common.Interfaces;

public interface ICacheQuery
{
    string Cachekey{get;}
    string[] Tags {get;}
    TimeSpan Expiration{get;}
}

public interface ICacheQuery<TResponse> : IRequest<TResponse>,ICacheQuery;
