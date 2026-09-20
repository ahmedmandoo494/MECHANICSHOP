using MechanicShop.Application.Features.RepairTasks.Mppers;
using MechanicShop.Domain.RepairTasks.Parts;
using MechanicShop.Tests.Common.RepaireTasks;
using Xunit;

namespace MechanicShop.Application.UnitTests.Mappers;

public class PartMapperTests
{
    [Fact]
    public void ToDto_Part()
    {
        // Given
        var part = PartFactory.CreatePart(
            name: "Brake Pad",
            cost: 100m,
            quantity: 2).Value;

        // When
        var dto = part.ToDto();

        // Then
        Assert.NotNull(dto);
        Assert.Equal(part.Id, dto.PartId);
        Assert.Equal(part.Name, dto.Name);
        Assert.Equal(part.Cost, dto.Cost);
        Assert.Equal(part.Quantity, dto.Quantity);
    }

    [Fact]
    public void ToDtos_Parts()
    {
        // Given
        var part1 = PartFactory.CreatePart(
            name: "Brake Pad",
            cost: 100m,
            quantity: 2).Value;

        var part2 = PartFactory.CreatePart(
            name: "Oil Filter",
            cost: 50m,
            quantity: 1).Value;

        var parts = new List<Part>
        {
            part1,
            part2
        };

        // When
        var dtos = parts.ToDtos();

        // Then
        Assert.NotNull(dtos);
        Assert.Equal(2, dtos.Count);

        Assert.Equal(part1.Id, dtos[0].PartId);
        Assert.Equal(part1.Name, dtos[0].Name);
        Assert.Equal(part1.Cost, dtos[0].Cost);
        Assert.Equal(part1.Quantity, dtos[0].Quantity);

        Assert.Equal(part2.Id, dtos[1].PartId);
        Assert.Equal(part2.Name, dtos[1].Name);
        Assert.Equal(part2.Cost, dtos[1].Cost);
        Assert.Equal(part2.Quantity, dtos[1].Quantity);
    }
}

