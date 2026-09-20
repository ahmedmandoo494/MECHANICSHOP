using MechanicShop.Domain.Employees;
using MechanicShop.Domain.Identity;
using MechanicShop.Tests.Common.Employees;

using Xunit;

namespace MechanicShop.Domain.UnitTests.Employees;

public class EmployeeTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var id = Guid.NewGuid();
        const string firstName = "John";
        const string lastName = "Doe";
        const Role role = Role.Labor;

        var result = EmployeeFactory.CreateEmployee(id: id, firstName: firstName, lastName: lastName, role: role);

        Assert.True(result.IsSuccess);
        var employee = result.Value;
        Assert.Equal(id, employee.Id);
        Assert.Equal(firstName, employee.FirstName);
        Assert.Equal(lastName, employee.LastName);
        Assert.Equal(role, employee.Role);
        Assert.Equal("John Doe", employee.FullName);
    }

    [Fact]
    public void Create_WithEmptyId_ShouldFail()
    {
        var result = Employee.Create(Guid.Empty, "John", "Doe", Role.Manager);

        Assert.True(result.IsError);
        Assert.Equal(EmployeeError.IdRequired.Code, result.TopError.Code);
        Assert.Equal(EmployeeError.IdRequired.Description, result.TopError.Description);
    }

    [Fact]
    public void Create_WithEmptyFirstName_ShouldFail()
    {
        var result = Employee.Create(Guid.NewGuid(), " ", "Doe", Role.Manager);

        Assert.True(result.IsError);
        Assert.Equal(EmployeeError.FirstNameIsRequired.Code, result.TopError.Code);
        Assert.Equal(EmployeeError.FirstNameIsRequired.Description, result.TopError.Description);
    }

    [Fact]
    public void Create_WithEmptyLastName_ShouldFail()
    {
        var result = Employee.Create(Guid.NewGuid(), "John", " ", Role.Manager);

        Assert.True(result.IsError);
        Assert.Equal(EmployeeError.LastNameIsRequired.Code, result.TopError.Code);
        Assert.Equal(EmployeeError.LastNameIsRequired.Description, result.TopError.Description);
    }

    [Fact]
    public void Create_WithInvalidRole_ShouldFail()
    {
        var result = Employee.Create(Guid.NewGuid(), "John", "Doe", (Role)999);

        Assert.True(result.IsError);
        Assert.Equal(EmployeeError.RoleInvalid.Code, result.TopError.Code);
        Assert.Equal(EmployeeError.RoleInvalid.Description, result.TopError.Description);
    }
}