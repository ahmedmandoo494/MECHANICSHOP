using FluentValidation;

namespace MechanicShop.Application.Features.WorkOrders.Commands.CreateWorkOrder;

public sealed class CreateWorkOrderCommandValidator : AbstractValidator<CreateWorkOrderCommand>
{
    public CreateWorkOrderCommandValidator()
    {
        RuleFor(w => w.VehicleId)
            .NotEmpty().WithMessage("VehicleId is required");
        RuleFor(w => w.LaborId)
            .NotEmpty().WithMessage("If provided, laberId must not be empty");
        RuleFor(w => w.RepairTaskIds)
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .NotEmpty().WithMessage("At least no repair task must be selected");
        RuleFor(w => w.Spot)
            .IsInEnum()
            .WithMessage("Spot must be a valid spot value. [A,B,C,D]");
        RuleFor(w => w.StartAt)
            .Must(startAt => startAt >= DateTimeOffset.UtcNow)
            .WithMessage("StartAt must be in the feature");
    }
}