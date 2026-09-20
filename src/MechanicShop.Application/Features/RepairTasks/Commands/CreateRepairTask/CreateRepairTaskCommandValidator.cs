using FluentValidation;
using MechanicShop.Application.Features.RepairTasks.Mppers;

namespace MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask;

public sealed class CreateRepairTaskCommandValidator : AbstractValidator<CreateRepairTaskCommand>
{
    public CreateRepairTaskCommandValidator()
    {
        RuleFor(r => r.Name)
        .NotEmpty().WithMessage("Name is required")
        .MaximumLength(100);

        RuleFor(r => r.LaborCost)
        .GreaterThan(0).WithMessage("labor cost must be greater than 0");

        RuleFor(r => r.RepairDurationInMinute)
        .NotNull().WithMessage("Estimated duration is required")
        .IsInEnum();

        RuleFor(r => r.Parts)
        .NotNull().WithMessage("Parts list cannot be null")
        .Must(p => p.Count > 0).WithMessage("At least one part is required");

        RuleForEach(r => r.Parts).SetValidator(new CreatePartCommandValidator());

    }
}


