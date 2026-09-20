

using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.RepairTasks.Parts;

public static class PartError
{
    public static readonly Error NameRequire =
        Error.Validation("Part.Name.Required","Part Name is Required");
    public static readonly Error CostInvalid =
        Error.Validation("Part.Cost.Invalid","Part Cost must between 1 and 10,000.");
    public static readonly Error QuantityInvalid =
        Error.Validation("Part.Quantity.Invalid","Part Quantity must between 1 and 10.");
}