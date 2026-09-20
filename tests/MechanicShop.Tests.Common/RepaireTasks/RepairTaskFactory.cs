using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Domain.RepairTasks.Enums;
using MechanicShop.Domain.RepairTasks.Parts;

namespace MechanicShop.Tests.Common.RepaireTasks;

public static class RepairTaskFactory
{
    public static Result<RepairTask> CreateRepairTask(
        Guid? id = null,
        string? name = null,
        decimal? laborCost = null,
        RepairDurationInMinute? repairDurationInMinutes = null,
        List<Part>? parts = null)
    {
        return RepairTask.Create(
            id ?? Guid.NewGuid(),
            name ?? "Brake Inspection",
            laborCost ?? 100,
            repairDurationInMinutes ?? RepairDurationInMinute.Min30,
            parts ?? [Part.Create(Guid.NewGuid(), "Brake Pads", 50, 1).Value]);
    }
}