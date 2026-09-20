using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.RepairTasks;

public static class RepairTaskError
{
    public static Error NameRequired =>
        Error.Validation(code:"RepairTask.Name.Required" , "RepairTasks Name Is Required");
    public static Error LaborCostInvalid =>
        Error.Validation("RepairTask.LaborCost.Invalid","Labor Cost must between 1 and 10,000.");
    public static Error DurationInvalid =>
        Error.Validation("RepairTask.Duration.Invalid","Duration must be between 15-180 Minutes");
    public static  Error PartsRequired =>
        Error.Validation("RepaitTask.Parts.Required","At Least one Part is Required");
    public static  Error PartNameRequired =>
        Error.Validation("RepaitTask.Parts.Name.Required","All Parts Must have a Name");
    public static Error AtLeastOneRepairTaskisRequired=>
        Error.Validation("RepairTask.Required","At Least One RepairTask must be specified.");
    public static Error InUse=>
        Error.Conflict("RepairTask.InUse","Cannot delete a repair task that is used in work order.");
    public static Error DuplicateName=>
        Error.Conflict("RepairTaskPart.Deplicate","A part With the Same name aleardy exists in this repair task.");
}