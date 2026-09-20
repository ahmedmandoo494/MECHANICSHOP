using System.ComponentModel.DataAnnotations;
using MechanicShop.Contracts.Common;

namespace MechanicShop.Contracts.Requests.RepairTasks;

public class CreateRepairTaskRequest
{
    [Required(ErrorMessage ="Name is required.")]
    public string? Name { get;  set; }

    [Required(ErrorMessage ="LaborCost is required.")]
    [Range(1,10000,ErrorMessage ="LaborCost must be between 1 to 10,000")]
    public decimal LaborCost { get;  set; }

    [Required(ErrorMessage ="Estimated duration is required.")]
    public RepairDurationInMinute? RepairDurationInMins { get;  set; }

    [MinLength(1,ErrorMessage ="At least one part is required.")]
    public List<CreatePartRequest> Parts { get;  set; } =[];
}
