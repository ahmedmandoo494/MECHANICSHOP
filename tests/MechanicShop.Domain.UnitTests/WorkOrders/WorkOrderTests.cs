using MechanicShop.Domain.WorkOrders;
using MechanicShop.Domain.WorkOrders.Enums;
using MechanicShop.Tests.Common.RepaireTasks;

using Xunit;

namespace MechanicShop.Domain.UnitTests.WorkOrders;

public class WorkOrderTests
{
    [Fact]
    public void Create_ShouldReturnError_WhenIdIsEmpty()
    {
        var wo = WorkOrder.Create(
                    id: Guid.Empty,
                    vehicleId: Guid.NewGuid(),
                    startAtUtc: DateTimeOffset.UtcNow,
                    endAtUtc: DateTimeOffset.UtcNow.AddHours(1),
                    laborId: Guid.NewGuid(),
                    spot: Spots.A,
                    repairTasks: [RepairTaskFactory.CreateRepairTask().Value]);

        Assert.False(wo.IsSuccess);

        Assert.Equal(WorkOrderErrors.WorkOrderIdRequired.Code, wo.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenVehicleIdIsEmpty()
    {
        var wo = WorkOrder.Create(
                           id: Guid.NewGuid(),
                           vehicleId: Guid.Empty,
                           startAtUtc: DateTimeOffset.UtcNow,
                           endAtUtc: DateTimeOffset.UtcNow.AddHours(1),
                           laborId: Guid.NewGuid(),
                           spot: Spots.A,
                           repairTasks: [RepairTaskFactory.CreateRepairTask().Value]);

        Assert.False(wo.IsSuccess);

        Assert.Equal(WorkOrderErrors.VehicleIdRequired.Code, wo.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenNoRepairTasks()
    {
        var wo = WorkOrder.Create(
                           id: Guid.NewGuid(),
                           vehicleId: Guid.NewGuid(),
                           startAtUtc: DateTimeOffset.UtcNow,
                           endAtUtc: DateTimeOffset.UtcNow.AddHours(1),
                           laborId: Guid.NewGuid(),
                           spot: Spots.A,
                           repairTasks: []);

        Assert.False(wo.IsSuccess);

        Assert.Equal(WorkOrderErrors.RepairTasksRequired.Code, wo.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenLaborIdIsEmpty()
    {
        var wo = WorkOrder.Create(
                              id: Guid.NewGuid(),
                              vehicleId: Guid.NewGuid(),
                              startAtUtc: DateTimeOffset.UtcNow,
                              endAtUtc: DateTimeOffset.UtcNow.AddHours(1),
                              laborId: Guid.Empty,
                              spot: Spots.A,
                              repairTasks: [RepairTaskFactory.CreateRepairTask().Value]);

        Assert.False(wo.IsSuccess);

        Assert.Equal(WorkOrderErrors.LaborIdRequired.Code, wo.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenTimingInvalid()
    {
        var wo = WorkOrder.Create(
                           id: Guid.NewGuid(),
                           vehicleId: Guid.NewGuid(),
                           startAtUtc: DateTimeOffset.UtcNow.AddHours(1),
                           endAtUtc: DateTimeOffset.UtcNow,
                           laborId: Guid.NewGuid(),
                           spot: Spots.A,
                           repairTasks: [RepairTaskFactory.CreateRepairTask().Value]);

        Assert.False(wo.IsSuccess);

        Assert.Equal(WorkOrderErrors.InvalidTiming.Code, wo.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenSpotInvalid()
    {
        const Spots invalidSpot = (Spots)999;

        var wo = WorkOrder.Create(
                      id: Guid.NewGuid(),
                      vehicleId: Guid.NewGuid(),
                      startAtUtc: DateTimeOffset.UtcNow,
                      endAtUtc: DateTimeOffset.UtcNow.AddHours(1),
                      laborId: Guid.NewGuid(),
                      spot: invalidSpot,
                      repairTasks: [RepairTaskFactory.CreateRepairTask().Value]);

        Assert.False(wo.IsSuccess);

        Assert.Equal(WorkOrderErrors.SpotInvalid.Code, wo.TopError.Code);
    }

    [Fact]
    public void AddRepairTask_ShouldReturnError_WhenNotEditable()
    {
        var wo = WorkOrder.Create(
                   id: Guid.NewGuid(),
                   vehicleId: Guid.NewGuid(),
                   startAtUtc: DateTimeOffset.UtcNow,
                   endAtUtc: DateTimeOffset.UtcNow.AddHours(1),
                   laborId: Guid.NewGuid(),
                   spot: Spots.A,
                   repairTasks: [RepairTaskFactory.CreateRepairTask().Value]).Value;

        wo.UpdateStatus(WorkOrderStatus.InProgress);
        wo.UpdateStatus(WorkOrderStatus.Completed);

        var result = wo.AddRepairTask(RepairTaskFactory.CreateRepairTask().Value);

        Assert.False(result.IsSuccess);
        Assert.True(result.Errors!.Count > 0);
    }

    [Fact]
    public void UpdateLabor_ShouldReturnError_WhenLaborIdEmpty()
    {
        var wo = WorkOrder.Create(
                       id: Guid.NewGuid(),
                       vehicleId: Guid.NewGuid(),
                       startAtUtc: DateTimeOffset.UtcNow,
                       endAtUtc: DateTimeOffset.UtcNow.AddHours(1),
                       laborId: Guid.NewGuid(),
                       spot: Spots.A,
                       repairTasks: [RepairTaskFactory.CreateRepairTask().Value]).Value;

        var result = wo.UpdateLabor(Guid.Empty);

        Assert.False(result.IsSuccess);
        Assert.Equal(WorkOrderErrors.LaborIdEmpty(wo.Id.ToString()).Code, result.TopError.Code);
    }

    [Fact]
    public void UpdateSpot_ShouldReturnError_WhenSpotInvalid()
    {
        var wo = WorkOrder.Create(
               id: Guid.NewGuid(),
               vehicleId: Guid.NewGuid(),
               startAtUtc: DateTimeOffset.UtcNow,
               endAtUtc: DateTimeOffset.UtcNow.AddHours(1),
               laborId: Guid.NewGuid(),
               spot: Spots.A,
               repairTasks: [RepairTaskFactory.CreateRepairTask().Value]).Value;

        const Spots invalidSpot = (Spots)999;
        var result = wo.UpdateSpot(invalidSpot);

        Assert.False(result.IsSuccess);
        Assert.Equal(WorkOrderErrors.SpotInvalid.Code, result.TopError.Code);
    }

    [Fact]
    public void UpdateTiming_ShouldReturnError_WhenInvalid()
    {
        var wo = WorkOrder.Create(
                          id: Guid.NewGuid(),
                          vehicleId: Guid.NewGuid(),
                          startAtUtc: DateTimeOffset.UtcNow,
                          endAtUtc: DateTimeOffset.UtcNow.AddHours(1),
                          laborId: Guid.NewGuid(),
                          spot: Spots.A,
                          repairTasks: [RepairTaskFactory.CreateRepairTask().Value]).Value;

        var result = wo.UpdateTime(DateTimeOffset.UtcNow.AddHours(2), DateTimeOffset.UtcNow);

        Assert.False(result.IsSuccess);
        Assert.Equal(WorkOrderErrors.InvalidTiming.Code, result.TopError.Code);
    }

    [Fact]
    public void UpdateState_ShouldReturnError_WhenTransitionInvalid()
    {
        var wo = WorkOrder.Create(
                      id: Guid.NewGuid(),
                      vehicleId: Guid.NewGuid(),
                      startAtUtc: DateTimeOffset.UtcNow,
                      endAtUtc: DateTimeOffset.UtcNow.AddHours(1),
                      laborId: Guid.NewGuid(),
                      spot: Spots.A,
                      repairTasks: [RepairTaskFactory.CreateRepairTask().Value]).Value;

        var result = wo.UpdateStatus(WorkOrderStatus.Completed);

        Assert.False(result.IsSuccess);
        Assert.Equal(WorkOrderErrors.InvalidStateTransition(WorkOrderStatus.Scheduled, WorkOrderStatus.Completed).Code, result.TopError.Code);
    }

    [Fact]
    public void UpdateLabor_ShouldReturnSuccess_AndSetNewLaborId()
    {
        var wo = WorkOrder.Create(
            id: Guid.NewGuid(),
            vehicleId: Guid.NewGuid(),
            startAtUtc: DateTimeOffset.UtcNow,
            endAtUtc: DateTimeOffset.UtcNow.AddHours(1),
            laborId: Guid.NewGuid(),
            spot: Spots.A,
            repairTasks: [RepairTaskFactory.CreateRepairTask().Value]).Value;

        var newLabor = Guid.NewGuid();
        var result = wo.UpdateLabor(newLabor);

        Assert.True(result.IsSuccess);
        Assert.Equal(newLabor, wo.LaborId);
    }

    [Fact]
    public void UpdateSpot_ShouldReturnSuccess_AndSetNewSpot()
    {
        var wo = WorkOrder.Create(
            id: Guid.NewGuid(),
            vehicleId: Guid.NewGuid(),
            startAtUtc: DateTimeOffset.UtcNow,
            endAtUtc: DateTimeOffset.UtcNow.AddHours(1),
            laborId: Guid.NewGuid(),
            spot: Spots.A,
            repairTasks: [RepairTaskFactory.CreateRepairTask().Value]).Value;

        var result = wo.UpdateSpot(Spots.B);

        Assert.True(result.IsSuccess);
        Assert.Equal(Spots.B, wo.Spot);
    }

    [Fact]
    public void UpdateTiming_ShouldReturnSuccess_AndSetNewTiming()
    {
        var wo = WorkOrder.Create(
            id: Guid.NewGuid(),
            vehicleId: Guid.NewGuid(),
            startAtUtc: DateTimeOffset.UtcNow,
            endAtUtc: DateTimeOffset.UtcNow.AddHours(1),
            laborId: Guid.NewGuid(),
            spot: Spots.A,
            repairTasks: [RepairTaskFactory.CreateRepairTask().Value]).Value;

        var newStart = wo.StartAtUtc.AddHours(2);
        var newEnd = newStart.AddHours(1);
        var result = wo.UpdateTime(newStart, newEnd);

        Assert.True(result.IsSuccess);
        Assert.Equal(newStart, wo.StartAtUtc);
        Assert.Equal(newEnd, wo.EndAtUtc);
    }

    [Fact]
    public void UpdateState_ShouldReturnSuccess_AndSetStateToInProgress()
    {
        var wo = WorkOrder.Create(
            id: Guid.NewGuid(),
            vehicleId: Guid.NewGuid(),
            startAtUtc: DateTimeOffset.UtcNow,
            endAtUtc: DateTimeOffset.UtcNow.AddHours(1),
            laborId: Guid.NewGuid(),
            spot: Spots.A,
            repairTasks: [RepairTaskFactory.CreateRepairTask().Value]).Value;

        var result = wo.UpdateStatus(WorkOrderStatus.InProgress);

        Assert.True(result.IsSuccess);
        Assert.Equal(WorkOrderStatus.InProgress, wo.Status);
    }
}