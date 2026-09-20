using FluentValidation;

namespace MechanicShop.Application.Features.RepairTasks.Commands.RemoveRepairTask;

public sealed class RemoveRepairTaskCommandValidator:AbstractValidator<RemoveRepairTaskCommand>
{
    public RemoveRepairTaskCommandValidator()
    {

        RuleFor(r => r.TaskId)
        .NotEmpty().WithMessage("Repair Task Id is required");
    }
}