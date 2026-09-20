using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Billing.Dtos;
using MechanicShop.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Billing.Queries.GetInvoicePdf;

public class GetInvoicePdfQureyHandler(
    ILogger<GetInvoicePdfQureyHandler> logger,
    IInvoicePdfGenerator pdfGenerator,
    IAppDbContext context
    )
    : IRequestHandler<GetInvoicePdfQuery, Result<InvoicePdfDto>>
{
    private readonly ILogger<GetInvoicePdfQureyHandler> _logger = logger;
    private readonly IInvoicePdfGenerator _pdfGenerator = pdfGenerator;
    private readonly IAppDbContext _context = context;


    public async Task<Result<InvoicePdfDto>> Handle(GetInvoicePdfQuery query, CancellationToken ct)
    {
        var invoice = await _context.Invoices.AsNoTracking()
            .Include(i => i.LineItems)
            .FirstOrDefaultAsync(i => i.Id == query.InvoiceId, ct);

        if(invoice is null)
        {
            _logger.LogInformation("Invoice with id {invoiceId} not found",query.InvoiceId);
            return ApplicationErrors.InvoiceNotFound;
        }
        try
        {
            var pdfByets = _pdfGenerator.Generator(invoice);
            var invoicePdf = new InvoicePdfDto
            {
                Content =pdfByets,
                FileName = $"Invoice-{invoice.Id}.pdf"
            };
            return invoicePdf;
        }
        catch (Exception ex) 
        {
            _logger.LogInformation(ex,"Falied to Generate PDF for Invoice id {invoiceId} not found",query.InvoiceId);
            return Error.Failure("An erro occured while generating the invoice PDF");
        }
    }
}