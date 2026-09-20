using MechanicShop.Domain.Identity;

namespace MechanicShop.Application.Features.Labors.Dtos;

public sealed class LaborDto
{
    public Guid LaborId{get;set;}
    public string? Name { get; set;}

}