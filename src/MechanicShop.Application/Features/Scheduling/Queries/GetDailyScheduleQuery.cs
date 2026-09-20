using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Scheduling.Dtos;
using MechanicShop.Domain.Common.Results;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Scheduling.Queries;

public sealed record GetDailyScheduleQuery(TimeZoneInfo TimeZone, DateOnly ScheduleDate, Guid? LaborId = null) : ICacheQuery<Result<ScheduleDto>>
{
    public string Cachekey => $"work-order:{ScheduleDate:yyyy-MM-dd}:labor={LaborId?.ToString() ?? "-"}";

    public string[] Tags => ["work-order"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
