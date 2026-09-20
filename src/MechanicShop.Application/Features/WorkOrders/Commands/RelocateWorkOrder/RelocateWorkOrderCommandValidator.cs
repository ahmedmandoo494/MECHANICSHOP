using FluentValidation;

namespace MechanicShop.Application.Features.WorkOrders.Commands.RelocateWorkOrder;

public sealed class RelocateWorkOrderCommandValidator : AbstractValidator<RelocateWorkOrderCommand>
{
    public RelocateWorkOrderCommandValidator()
    {
        RuleFor(w => w.WorkOrderId)
            .NotEmpty()
            .WithErrorCode("WorkOrder_Id_Required")
            .WithMessage("WorkOrderId is required");
        RuleFor(w => w.NewStartAt)
            .GreaterThan(DateTimeOffset.UtcNow)
            .WithMessage("New Start time must be in the feature.");

        RuleFor(s => s.Spot).IsInEnum();
    }
}
