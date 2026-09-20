using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.RepairTasks.Enums;
using MechanicShop.Domain.RepairTasks.Parts;

namespace MechanicShop.Domain.RepairTasks;

public sealed class RepairTask : AuditableEntity
{
    public string? Name { get; private set; }
    public decimal LaborCost { get; private set; }
    public RepairDurationInMinute RepairDurationInMinute { get; private set; }

    private List<Part> _parts =[];
    public IEnumerable<Part> Parts => _parts.AsReadOnly();
    public decimal TotalCost => LaborCost + _parts.Sum(p => p.Cost * p.Quantity);
    private RepairTask()
    {
        
    }

    public RepairTask(Guid id,string? name, decimal laborCost, RepairDurationInMinute repairDurationInMinute, List<Part> parts):base(id)
    {
        Name = name;
        LaborCost = laborCost;
        RepairDurationInMinute = repairDurationInMinute;
        _parts = parts;
    }

    public static Result<RepairTask> Create(Guid id,string? name, decimal laborCost, RepairDurationInMinute repairDurationInMinute, List<Part> parts)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return RepairTaskError.NameRequired;
        }
        if(laborCost <= 0)
        {
            return RepairTaskError.LaborCostInvalid;
        }
        if (!Enum.IsDefined(repairDurationInMinute))
        {
            return RepairTaskError.DurationInvalid;
        }

        if (parts is null || parts.Count == 0)
        {
            return RepairTaskError.PartsRequired;
        }

        return new RepairTask(id,name?.Trim(),laborCost,repairDurationInMinute,parts);
    }
    public Result<Updated> Update(string? name, decimal laborCost, RepairDurationInMinute repairDurationInMinute)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return RepairTaskError.NameRequired;
        }
        if(laborCost <= 0 || laborCost > 10000)
        {
            return RepairTaskError.LaborCostInvalid;
        }
        if (!Enum.IsDefined(repairDurationInMinute))
        {
            return RepairTaskError.DurationInvalid;
        }
        Name = name.Trim();
        LaborCost = laborCost;
        RepairDurationInMinute = repairDurationInMinute;

        return Result.Updated;
    }

    public Result<Updated> UpsertParts(List<Part> IncomingParts)
    {
        _parts.RemoveAll(existing => IncomingParts.All( p => p.Id != existing.Id));

        foreach (var part in IncomingParts)
        {
            var existing = _parts.FirstOrDefault(p => p.Id == part.Id);
            if(existing is null)
            {
                _parts.Add(part);
            }
            else
            {
                var UpdatePartResult = existing.Update(part.Name,part.Cost,part.Quantity);
                if (UpdatePartResult.IsError)
                {
                    return UpdatePartResult.Errors!;
                }
            }
        }
        return Result.Updated;
    }
}


