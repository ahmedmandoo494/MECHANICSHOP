using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Billing.Commands.SettleInvoice;

public sealed class SettleInvoiceCommandHandler(IAppDbContext context, ILogger<SettleInvoiceCommandHandler> logger,
    HybridCache cache, TimeProvider datetime):IRequestHandler<SettleInvoiceCommand,Result<Success>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<SettleInvoiceCommandHandler> _logger = logger;
    private readonly HybridCache _cache = cache;
    private readonly TimeProvider _datetime = datetime;  

    public async Task<Result<Success>> Handle(SettleInvoiceCommand command, CancellationToken ct)
    {
        var invoice =await _context.Invoices.FirstOrDefaultAsync(i => i.Id == command.InvoiceId , ct);
        if(invoice is null)
        {
            _logger.LogInformation("Invoice whti id {invoiceId} not found",command.InvoiceId);
            return ApplicationErrors.InvoiceNotFound;
        }

        var payInvoiceResult = invoice.MarkAsPaid(_datetime);
        if (payInvoiceResult.IsError)
        {
            _logger.LogInformation("Invoice Payment failed for InvoiceId : {workOrderId}. Errors: {@error}",command.InvoiceId,payInvoiceResult.Errors);
            return payInvoiceResult.Errors!;
        }
        await _context.SaveChangesAsync(ct);
        await _cache.RemoveAsync("invoice",ct);
        _logger.LogInformation("Invoice {invoiceId} Successfuly Paid.",invoice.Id);

        return Result.Success;
    }
}