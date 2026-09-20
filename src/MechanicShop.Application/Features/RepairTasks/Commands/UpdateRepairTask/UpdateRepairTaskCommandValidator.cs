using FluentValidation;
using MechanicShop.Application.Features.RepairTasks.Mppers;

namespace MechanicShop.Application.Features.RepairTasks.Commands.UpdateRepairTask;

public sealed class UpdateRepairTaskCommandValidator : AbstractValidator<UpdateRepairTaskCommand>
{
    public UpdateRepairTaskCommandValidator()
    {

        RuleFor(r => r.TaskId)
        .NotEmpty().WithMessage("Task Id is required");

        RuleFor(r => r.Name)
        .NotEmpty().WithMessage("Task Name is required")
        .MaximumLength(100);

        RuleFor(r => r.LaborCost)
        .InclusiveBetween(1,10_000).WithMessage("Labor cost must be Between 1 and 10_000.");

        RuleFor(r => r.RepairDurationInMinute)
        .IsInEnum().WithMessage("Invalid duration selected.");

        RuleFor(r => r.Parts)
        .NotNull()
        .Must(p => p.Count > 0).WithMessage("At least one part is required");

        RuleForEach(r => r.Parts).SetValidator(new UpdatePartCommandValidator());

    }
}


