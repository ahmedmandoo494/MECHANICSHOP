using FluentValidation;

namespace MechanicShop.Application.Features.WorkOrders.Commands.AssingLabor;

public sealed class AssignLaborCommandValidator : AbstractValidator<AssignLaborCommand>
{
    public AssignLaborCommandValidator()
    {
        RuleFor(w => w.WorkOrderId)
            .NotEmpty()
            .WithErrorCode("WorkOrder_Id_Required")
            .WithMessage("WorkOrderId is required");
        RuleFor(w => w.LaborId)
            .NotEmpty()
            .WithErrorCode("Labor_Id_Required")
            .WithMessage("If provided, laberId must not be empty");
    }
}
