using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Billing.Dtos;
using MechanicShop.Application.Features.Billing.Mappers;
using MechanicShop.Domain.Common.Constants;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.RepairTasks.Parts;
using MechanicShop.Domain.WorkOrders;
using MechanicShop.Domain.WorkOrders.Billing;
using MechanicShop.Domain.WorkOrders.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace MechanicShop.Application.Features.Billing.Commands.IssueInvoice;

public sealed class IssueInvoiceCommandHandler(IAppDbContext context, ILogger<IssueInvoiceCommandHandler> logger,
    HybridCache cache, TimeProvider datetime) : IRequestHandler<IssueInvoiceCommand, Result<InvoiceDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<IssueInvoiceCommandHandler> _logger = logger;
    private readonly HybridCache _cache = cache;
    private readonly TimeProvider _datetime = datetime;

    public async Task<Result<InvoiceDto>> Handle(IssueInvoiceCommand command, CancellationToken ct)
    {
        var workOrder = await _context.WorkOrders
        .Include(w => w.Vehicle!).ThenInclude(v => v.Customer)
        .Include(w => w.RepairTasks).ThenInclude(r => r.Parts)
        .FirstOrDefaultAsync(w => w.Id == command.WorktOrderId , ct);

        if(workOrder is null)
        {
            _logger.LogInformation("Invoice issuance failed,Work Order with id {workorderid} not found",command.WorktOrderId);
            return ApplicationErrors.WorkOrderNotFound;
        }

        if(workOrder.Status != WorkOrderStatus.Completed)
        {
            _logger.LogInformation("Invoice issuance rejected. workOrder {workOrderid} is not in completed",command.WorktOrderId);
            return ApplicationErrors.WorkOrderMustBeCompletedForInvoicing;
        }
        // var existingInvoice = await _context.Invoices
        //     .FirstOrDefaultAsync(i => i.WorkOrderId == workOrder.Id, ct);

        //         if (existingInvoice is not null)
        //         {
        //             _logger.LogInformation(
        //                 "Invoice already exists for workOrder {workOrderId}.",
        //                 workOrder.Id);

        //             return ApplicationErrors.InvoiceAlreadyExists;
        //         }
        var invoiceId = Guid.NewGuid();

        var lineItems = new List<InvoiceLineItem>();

        var lineNumber = 1;

        foreach(var (task, taskIndex) in workOrder.RepairTasks.Select((t,i) => (t, i + 1)))
        {
            var partsSummary = task.Parts.Any()
            ?string.Join(Environment.NewLine,task.Parts.Select(p => $"    | {p.Name} x{p.Quantity} @{p.Cost}"))
            :"    | No parts";

            var lineDescription = 
            $"{taskIndex}: {task.Name}{Environment.NewLine}"+
            $"  Labor : {task.LaborCost}{Environment.NewLine}"+
            $"  parts :{Environment.NewLine}{partsSummary}";

            var totalPartsCost = task.Parts.Sum(p => p.Cost *p.Quantity);
            var totalTaskCost = task.LaborCost + totalPartsCost;

            var lineItemResult = InvoiceLineItem.Create(
                invoiceId,
                lineNumber++,
                lineDescription,
                1,
                totalTaskCost);
            if (lineItemResult.IsError)
            {
                return lineItemResult.Errors!;
            }
            lineItems.Add(lineItemResult.Value);
        }
        var subTotal = lineItems.Sum(l => l.LineTotal);
        var taxAmount = subTotal * MechanicShopConstants.TaxRate;
        var discountAmount = workOrder.Discount ?? 0m;

        var createInvoiceResult = Invoice.Create(
            invoiceId,
            workOrder.Id,
            _datetime,
            discountAmount,
            taxAmount,
            lineItems
        );
        if (createInvoiceResult.IsError)
        {
            _logger.LogInformation("Invoice creation failed for workOrderId: {workOrderId}. Errors: {@error}",command.WorktOrderId,createInvoiceResult.Errors);
            return createInvoiceResult.Errors!;
        }
        var invoice = createInvoiceResult.Value;

        _context.Invoices.Add(invoice);

        await _context.SaveChangesAsync(ct);

        await _cache.RemoveAsync("invoice",ct);

        _logger.LogInformation("Invoice {invoiceId} issued for workOrder {workOrderId}.",invoice.Id,workOrder.Id);
        return invoice.ToDto();

    }
}