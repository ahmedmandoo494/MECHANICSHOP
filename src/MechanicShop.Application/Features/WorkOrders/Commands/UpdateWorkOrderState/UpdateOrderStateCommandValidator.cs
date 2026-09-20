using FluentValidation;

namespace MechanicShop.Application.Features.WorkOrders.Commands.UpdateWorkOrderState;

public sealed class UpdateOrderStateCommandValidator : AbstractValidator<UpdateOrderStateCommand>
{
    public UpdateOrderStateCommandValidator()
    {
        RuleFor(w => w.WorkOrderId)
            .NotEmpty()
            .WithErrorCode("WorkOrder_Id_Required")
            .WithMessage("WorkOrderId is required");
        RuleFor(w => w.State)
            .IsInEnum()
            .WithErrorCode("WorkOrderStatus_Is_Required")
            .WithMessage("State must be a valid WorkOrderStatus value.");
    }
}
