using System.ComponentModel.DataAnnotations;

namespace MechanicShop.Contracts.Requests.WorkOrders;

public record AssignLaborRequest
{
    [Required(ErrorMessage = "LaborId is required.")]
    public string LaberId { get; set; } = string.Empty;
}