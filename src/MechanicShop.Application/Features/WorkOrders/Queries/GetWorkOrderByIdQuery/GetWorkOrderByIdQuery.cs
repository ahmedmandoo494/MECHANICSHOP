using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.WorkOrders.Dtos;
using MechanicShop.Domain.Common.Results;


namespace MechanicShop.Application.Features.WorkOrders.Queries.GetWorkOrderByIdQuery;

public sealed record GetWorkOrderByIdQuery(Guid WorkOrderId) : ICacheQuery<Result<WorkOrderDto>>
{
    public string Cachekey => $"work-order:{WorkOrderId}";
    public string[] Tags => ["work-order"];
    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}