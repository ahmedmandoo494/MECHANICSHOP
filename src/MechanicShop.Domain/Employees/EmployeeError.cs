using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.Employees;

public static class EmployeeError
{
    public static Error IdRequired =>
        Error.Validation("Employee.Id.Required","Id is Required");
    public static Error FirstNameIsRequired =>
        Error.Validation("Employee.FirstName.Required","First Name is Required");
    public static Error LastNameIsRequired =>
        Error.Validation("Employee.LastName.Required","Last Name is Required");
    public static Error RoleInvalid =>
        Error.Validation("Employee.Role.Invalid","Invalid Role Assigned to employee");
}