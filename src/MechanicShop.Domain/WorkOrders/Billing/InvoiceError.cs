using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.WorkOrders.Billing;

public static class InvoiceError
{
    public static readonly Error WorkOrderInvalid = Error.Validation(
        "Invoice.WorkOrder.Invalid","WorkOrder is invalid"
    );
    public static readonly Error LineItemEmpty = Error.Validation(
        "Invoice.LineItme.Empty","Invoice must have line items."
    );
    public static readonly Error InvoiceLocked = Error.Validation(
        "Invoice.Locked","Invoice is locked"
    );
    public static readonly Error DiscountNegative = Error.Validation(
        "Invoice.Discount.Negative","Discount cannot be Negative"
    );
    public static readonly Error DiscountExceedsSubTotal = Error.Validation(
        "Invoice.Discount.ExceedsSubTotal","Discount Exceeds SubTotal"
    );
}