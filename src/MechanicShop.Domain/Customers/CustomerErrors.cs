

using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.Customers;

public static class CustomerErrors
{
    public static Error NameRequired =>
        Error.Validation(code:"Customer_Name_Required" , "Customer Name Is Required");
    public static Error PhoneNumberRequired =>
        Error.Validation(code:"Customer_PhoneNumber_Required" , "Customer PhoneNumber Is Required");
    public static Error EmailRequired =>
        Error.Validation(code:"Customer_Email_Required" , "Customer Email Is Required");
    public static Error EmailInvalid =>
        Error.Validation(code:"Customer_Email_Invalid" , "Customer Email Is Invalid");
    public static Error CustomerExists => 
        Error.Conflict(code:"Customer_Email_Exists",description:"A customer with this email already exist.");
    public static readonly Error InvalidPhoneNumber =
        Error.Conflict("Customer.InvalidPhoneNumber","Phone number must have 7-15 digits and may start with '+'.");
    public static readonly Error CannontDeleteCustomerWithWorkOrder =
        Error.Conflict("Customer.cannotDelete","Customer cannot deleted due to existing work orders.");
}