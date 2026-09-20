using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.WorkOrders.Billing;

public class Invoice:AuditableEntity
{
    public Guid WorkOrderId { get; }
    public DateTimeOffset IssuedAtUtc { get; }
    public decimal DiscountAmount { get; private set; }
    public decimal TaxAmount  { get;  }
    public decimal SubTotal => _lineItems.Sum(li => li.LineTotal);
    public decimal Total => SubTotal -DiscountAmount + TaxAmount;
    public DateTimeOffset? PaidAt { get;private set; }
    public WorkOrder? WorkOrder { get;private set; }
    private readonly List<InvoiceLineItem> _lineItems = [];
    public IEnumerable<InvoiceLineItem> LineItems => _lineItems.AsReadOnly();
    public InvoiceStatus Status { get;private set; }

    private Invoice(){}

    public Invoice(
        Guid id,
        Guid workOrderId,
        DateTimeOffset issuedAtUtc,
        decimal discountAmount,
        decimal taxAmount,
        List<InvoiceLineItem> lineItems)
        :base(id)
    {
        WorkOrderId = workOrderId;
        IssuedAtUtc = issuedAtUtc;
        DiscountAmount = discountAmount;
        TaxAmount = taxAmount;
        Status = InvoiceStatus.Unpaid;
        _lineItems = lineItems;
    }

    public static Result<Invoice> Create(
        Guid id,
        Guid workOrderId,
        TimeProvider datetime,
        decimal discountAmount,
        decimal taxAmount,
        List<InvoiceLineItem> lineItems)
    {
        if(workOrderId == Guid.Empty)
        {
            return InvoiceError.WorkOrderInvalid;
        }
        if(lineItems is null || lineItems.Count == 0)
        {
            return InvoiceError.LineItemEmpty;
        }
        return new Invoice(id,workOrderId,datetime.GetUtcNow(),discountAmount,taxAmount,lineItems);
    }

    public Result<Updated> ApplayDiscount(decimal discountAmount)
    {
        if(Status != InvoiceStatus.Unpaid)
        {
            return InvoiceError.InvoiceLocked;
        }
        if(discountAmount < 0)
        {
            return InvoiceError.DiscountNegative;
        }
        if(discountAmount > SubTotal)
        {
            return InvoiceError.DiscountExceedsSubTotal;
        }
        DiscountAmount = discountAmount;
        return Result.Updated;
    }

    public Result<Updated> MarkAsPaid(TimeProvider timeProvider)
    {
        if(Status != InvoiceStatus.Unpaid)
        {
            return InvoiceError.InvoiceLocked;
        }
        Status = InvoiceStatus.Paid;
        PaidAt = timeProvider.GetUtcNow();
        return Result.Updated;
    }
}
