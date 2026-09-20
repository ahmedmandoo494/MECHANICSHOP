using FluentValidation;

namespace MechanicShop.Application.Features.Billing.Commands.SettleInvoice;

public sealed class SettleInvoiceCommandValidator : AbstractValidator<SettleInvoiceCommand>
{
    public SettleInvoiceCommandValidator()
    {
        RuleFor(i => i.InvoiceId)
        .NotEmpty()
        .WithErrorCode("InvoiceId_Is_Required")
        .WithMessage("Invoice id is required.");
    }
}
