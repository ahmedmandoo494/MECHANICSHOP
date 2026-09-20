using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.WorkOrders.Billing;

public static class InvoiceLineItemError
{
    public static Error InvoiceIdRequired => 
        Error.Validation("InvoiceLineItemError.InvoiceIdRequired","InvoiceId Is Required");
    public static Error LineNumberRequired => 
        Error.Validation("InvoiceLineItemError.LineNumberRequired","LineNumber Is Required");
    public static Error DescriptionRequired => 
        Error.Validation("InvoiceLineItemError.DescriptionRequired","Descripion Is Required");
    public static Error QuantityInvalid => 
        Error.Conflict("InvoiceLineItemError.QuantityInvalid", "Quantity must be greater than 0.");
    public static Error UnitPriceInvalid => 
        Error.Conflict("InvoiceLineItemError.UnitPriceInvalid", "UnitPrice must be greater than 0.");
}
