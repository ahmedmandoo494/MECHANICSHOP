using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Employees;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Domain.Vehicles;
using MechanicShop.Domain.WorkOrders.Billing;
using MechanicShop.Domain.WorkOrders.Enums;
using Microsoft.IdentityModel.Tokens;

namespace MechanicShop.Domain.WorkOrders;

public sealed class WorkOrder : AuditableEntity
{
    public Guid VehicleId { get; }
    public DateTimeOffset StartAtUtc { get; private set; }
    public DateTimeOffset EndAtUtc { get; private set; }
    public Guid LaborId { get; private set;}
    public Spots Spot { get; private set; }
    public WorkOrderStatus Status { get; private set; }
    public Employee? Labor{get;  set;}
    public Vehicle? Vehicle { get;  set; }
    public Invoice? Invoice {get;  set;}
    public decimal? Discount { get; set; }
    public decimal? Tax { get; private set; }
    public decimal? TotalPartCost =>  _repairTasks.SelectMany(rt => rt.Parts).Sum(p => p.Cost * p.Quantity);
    public decimal? TotalLaborCost =>  _repairTasks.Sum(rt => rt.LaborCost);
    public decimal? Total => (TotalPartCost ?? 0) + (TotalLaborCost ?? 0);

    private List<RepairTask> _repairTasks= [];

    public IEnumerable<RepairTask> RepairTasks => _repairTasks.AsReadOnly(); 
    private WorkOrder()
    {
    }

    private WorkOrder(Guid id, Guid vehicleId, DateTimeOffset startAtUtc, DateTimeOffset endAtUtc, Guid laborId, Spots spot, WorkOrderStatus status,List<RepairTask> repairTasks)
    :base(id)
    {
        VehicleId = vehicleId;
        StartAtUtc = startAtUtc;
        EndAtUtc = endAtUtc;
        LaborId = laborId;
        Spot = spot;
        Status = status;
        _repairTasks =repairTasks;
    }
    public static Result<WorkOrder> Create(Guid id, Guid vehicleId, DateTimeOffset startAtUtc, DateTimeOffset endAtUtc, Guid laborId, Spots spot,List<RepairTask> repairTasks)
    {
        if(id == Guid.Empty)
        {
            return WorkOrderErrors.WorkOrderIdRequired;
        }
        if(vehicleId == Guid.Empty)
        {
            return WorkOrderErrors.VehicleIdRequired;
        }
        if(repairTasks.Count == 0)
        {
            return WorkOrderErrors.RepairTasksRequired;
        }
        if(laborId == Guid.Empty)
        {
            return WorkOrderErrors.LaborIdRequired;
        }
        if(endAtUtc <= startAtUtc)
        {
            return WorkOrderErrors.InvalidTiming;
        }
        if (!Enum.IsDefined(spot))
        {
            return WorkOrderErrors.SpotInvalid;
        }
        return new WorkOrder(id,vehicleId,startAtUtc,endAtUtc,laborId,spot,WorkOrderStatus.Scheduled,repairTasks);
    }

    public bool IsEditable => Status is not (WorkOrderStatus.Completed or WorkOrderStatus.InProgress or WorkOrderStatus.Cancelled);
    public bool CanTransitionTo(WorkOrderStatus nextState)
    {
        return (Status,nextState) switch
        {
            (WorkOrderStatus.Scheduled,WorkOrderStatus.InProgress) => true,
            (WorkOrderStatus.InProgress,WorkOrderStatus.Completed) => true,
            (_,WorkOrderStatus.Cancelled) when Status != WorkOrderStatus.Completed => true,
            _=>false
        };
    }

    public Result<Updated> AddRepairTask(RepairTask repairTask)
    {
        if (!IsEditable)
        {
            return WorkOrderErrors.Readonly;
        }
        if(repairTask is null)
        {
                return WorkOrderErrors.RepairTasksRequired;
        }
        if(_repairTasks.Any(rt=> rt.Id == repairTask.Id))
        {
                return WorkOrderErrors.RepairTaskAlreadyAdded;
        }
        _repairTasks.Add(repairTask);
        return Result.Updated;
    }
    public Result<Updated> UpdateTime(DateTimeOffset startAtUtc, DateTimeOffset endAtUtc)
    {
        if (!IsEditable)
        {
            return WorkOrderErrors.Readonly;
        }
        if(endAtUtc <= startAtUtc)
        {
            return WorkOrderErrors.InvalidTiming;
        }
        StartAtUtc = startAtUtc;
        EndAtUtc = endAtUtc;
        return Result.Updated;
    }

    public Result<Updated> UpdateLabor(Guid laborId)
    {
        if (!IsEditable)
        {
            return WorkOrderErrors.Readonly;
        }
        if(laborId == Guid.Empty)
        {
            return WorkOrderErrors.LaborIdEmpty(laborId.ToString());
        }
        LaborId = laborId;

        return Result.Updated;
    }
    public Result<Updated> UpdateStatus(WorkOrderStatus newState)
    {

        if(!CanTransitionTo(newState))
        {
            return WorkOrderErrors.InvalidStateTransition(Status,newState);
        }
        Status = newState;

        return Result.Updated;
    }
    public Result<Updated> Cancel()
    {

        if(!CanTransitionTo(WorkOrderStatus.Cancelled))
        {
            return WorkOrderErrors.InvalidStateTransition(Status,WorkOrderStatus.Cancelled);
        }
        Status = WorkOrderStatus.Cancelled;

        return Result.Updated;
    }
    public Result<Updated> ClearRepairTask()
    {
        if (!IsEditable)
        {
            return WorkOrderErrors.Readonly;
        }
        _repairTasks.Clear();
        return Result.Updated;
    }

    public Result<Updated> UpdateSpot(Spots spot)
    {

        if (!IsEditable)
        {
            return WorkOrderErrors.Readonly;
        }
        if(!Enum.IsDefined(spot))
        {
            return WorkOrderErrors.SpotInvalid;  
        }
        Spot =spot;

        return Result.Updated;
    }
}
