using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.Customers.Vehicles;

public static class VehicleErrors
{
    public static Error CustomerIdRequired =>
        Error.Validation(code:"Vehicle_Customer_Required" , "Vehicle Customer Is Required");

    public static Error MakeRequired =>
        Error.Validation(code:"Vehicle_Make_Required" , "Vehicle Make Is Required");

    public static Error ModelRequired =>
        Error.Validation(code:"Vehicle_Model_Required" , "Vehicle Model Is Required");
    public static Error LicensePlateRequired =>
        Error.Validation(code:"Vehicle_LicensePlate_Required" , "Vehicle LicensePlate Is Required");
    public static Error YearInvalid =>
        Error.Validation(code:"Vehicle_Year_Invalid" , "Year must be between 1886 and next year.");
}