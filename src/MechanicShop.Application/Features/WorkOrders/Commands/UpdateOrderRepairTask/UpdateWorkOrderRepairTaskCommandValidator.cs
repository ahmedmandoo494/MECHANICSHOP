using FluentValidation;

namespace MechanicShop.Application.Features.WorkOrders.Commands.UpdateOrderRepairTask;

public sealed class UpdateWorkOrderRepairTaskCommandValidator : AbstractValidator<UpdateWorkOrderRepairTaskCommand>
{
    public UpdateWorkOrderRepairTaskCommandValidator()
    {
        RuleFor(w => w.WorkOrderId)
            .NotEmpty()
            .WithErrorCode("WorkOrder_Id_Required")
            .WithMessage("");
        RuleFor(w => w.RepairTaskIds)
            .NotEmpty()
            .WithErrorCode("RepairTaskIds_Is_Required")
            .WithMessage("At least one Repair Task is required.");
    }
}
