
using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Identity;

namespace MechanicShop.Domain.Employees;

public sealed class Employee:AuditableEntity
{
    public string? FirstName { get; }
    public string? LastName { get; }
    public Role Role { get; }

    public string FullName => $"{FirstName} {LastName}";

    private Employee()
    {
    }

    private Employee(Guid id,string? firstName, string? lastName, Role role):base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Role = role;
    }

    public static Result<Employee> Create(Guid id,string? firstName, string? lastName, Role role)
    {
        if(id == Guid.Empty)
        {
            return EmployeeError.IdRequired;
        }
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return EmployeeError.FirstNameIsRequired;
        }
        if (string.IsNullOrWhiteSpace(lastName))
        {
            return EmployeeError.LastNameIsRequired;
        }

        if (!Enum.IsDefined(role))
        {
            return EmployeeError.RoleInvalid;
        }
        return  new Employee(id,firstName.Trim(),lastName.Trim(),role);
    }
}