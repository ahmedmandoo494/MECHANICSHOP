using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Labors.Dtos;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Features.Labors.Queries;

public sealed class GetLaborQuery : ICacheQuery<Result<List<LaborDto>>>
{
    public string Cachekey => $"labors";

    public string[] Tags => ["labors"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}