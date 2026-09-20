using FluentValidation;

namespace MechanicShop.Application.Features.RepairTasks.Commands.UpdateRepairTask;

public sealed class UpdatePartCommandValidator : AbstractValidator<UpdatePartsCommand>
{
    public UpdatePartCommandValidator()
    {
        RuleFor(p => p.Name)
        .NotEmpty().WithMessage("Part Name is required")
        .MaximumLength(100);

        RuleFor(p => p.Cost)
        .InclusiveBetween(1,10_000).WithMessage("Part cost must be Between 1 and 10_000.");

        RuleFor(p => p.Quantity)
        .InclusiveBetween(1,10).WithMessage("Quantity must be between 1 and 10.");
    }
}