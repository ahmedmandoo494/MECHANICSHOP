using FluentValidation;

namespace MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask;

public sealed class CreatePartCommandValidator : AbstractValidator<CreatePartsCommand>
{
    public CreatePartCommandValidator()
    {
        RuleFor(p => p.Name)
        .NotEmpty().WithMessage("Part Name is required")
        .MaximumLength(100);

        RuleFor(p => p.Cost)
        .GreaterThan(0).WithMessage("Part cost must be greater than 0");

        RuleFor(p => p.Quantity)
        .GreaterThan(0).WithMessage("Quantity must be greater than 0");
    }
}