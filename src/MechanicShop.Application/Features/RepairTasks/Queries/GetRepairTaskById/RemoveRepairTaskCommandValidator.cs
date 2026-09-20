using FluentValidation;
using MechanicShop.Application.Features.RepairTasks.Queries.GetRepairTaskById;

namespace MechanicShop.Application.Features.RepairTasks.Commands.RemoveRepairTask;

public sealed class GetRepairTaskByIdQueryValidator:AbstractValidator<GetRepairTaskByIdQuery>
{
    public GetRepairTaskByIdQueryValidator()
    {

        RuleFor(r => r.RepairTaskId)
        .NotEmpty()
        .WithErrorCode("RepairTask_Is_Required")
        .WithMessage("Repair Task Id is required");
    }
}