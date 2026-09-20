using MechanicShop.Application.Features.RepairTasks.Mppers;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Domain.RepairTasks.Enums;
using MechanicShop.Tests.Common.RepaireTasks;
using Xunit;

namespace MechanicShop.Application.UnitTests.Mappers;


public class RepairTaskMapperTests
{
    [Fact]
    public void ToDto_RepairTask()
    {
        // Given
        var part = PartFactory.CreatePart(
            name: "Brake Pad",
            cost: 100m,
            quantity: 2).Value;

        var repairTask = RepairTaskFactory.CreateRepairTask(
            name: "Brake Inspection",
            laborCost: 150m,
            repairDurationInMinutes: RepairDurationInMinute.Min30,
            parts: [part]).Value;

        // When
        var dto = repairTask.ToDto();

        // Then
        Assert.NotNull(dto);
        Assert.Equal(repairTask.Id, dto.RepairTaskId);
        Assert.Equal(repairTask.Name, dto.Name);
        Assert.Equal(repairTask.LaborCost, dto.LaborCost);
        Assert.Equal(repairTask.TotalCost, dto.TotalCost);
        Assert.Equal(
            repairTask.RepairDurationInMinute,
            dto.RepairDurationInMinute);

        Assert.Single(dto.Parts);

        var partDto = dto.Parts[0];

        Assert.Equal(part.Id, partDto.PartId);
        Assert.Equal(part.Name, partDto.Name);
        Assert.Equal(part.Cost, partDto.Cost);
        Assert.Equal(part.Quantity, partDto.Quantity);
    }

    [Fact]
    public void ToDtos_RepairTasks()
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

        var repairTask1 = RepairTaskFactory.CreateRepairTask(
            name: "Brake Inspection",
            laborCost: 150m,
            repairDurationInMinutes: RepairDurationInMinute.Min30,
            parts: [part1]).Value;

        var repairTask2 = RepairTaskFactory.CreateRepairTask(
            name: "Oil Change",
            laborCost: 100m,
            repairDurationInMinutes: RepairDurationInMinute.Min30,
            parts: [part2]).Value;

        var repairTasks = new List<RepairTask>
        {
            repairTask1,
            repairTask2
        };

        // When
        var dtos = repairTasks.ToDtos();

        // Then
        Assert.NotNull(dtos);
        Assert.Equal(2, dtos.Count);

        Assert.Equal(repairTask1.Id, dtos[0].RepairTaskId);
        Assert.Equal(repairTask1.Name, dtos[0].Name);
        Assert.Equal(repairTask1.LaborCost, dtos[0].LaborCost);
        Assert.Equal(repairTask1.TotalCost, dtos[0].TotalCost);
        Assert.Equal(
            repairTask1.RepairDurationInMinute,
            dtos[0].RepairDurationInMinute);

        Assert.Single(dtos[0].Parts);
        Assert.Equal(part1.Id, dtos[0].Parts[0].PartId);

        Assert.Equal(repairTask2.Id, dtos[1].RepairTaskId);
        Assert.Equal(repairTask2.Name, dtos[1].Name);
        Assert.Equal(repairTask2.LaborCost, dtos[1].LaborCost);
        Assert.Equal(repairTask2.TotalCost, dtos[1].TotalCost);
        Assert.Equal(
            repairTask2.RepairDurationInMinute,
            dtos[1].RepairDurationInMinute);

        Assert.Single(dtos[1].Parts);
        Assert.Equal(part2.Id, dtos[1].Parts[0].PartId);
    }
}

