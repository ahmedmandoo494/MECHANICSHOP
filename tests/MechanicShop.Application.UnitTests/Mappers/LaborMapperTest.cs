using MechanicShop.Application.Features.Labors.Mappers;
using MechanicShop.Domain.Employees;
using MechanicShop.Tests.Common.Employees;
using Xunit;

namespace MechanicShop.Application.UnitTests.Mappers;


public class LaborMapperTests
{
    [Fact]
    public void ToDto_Employee()
    {
        // Given
        var labor = EmployeeFactory.CreateLabor().Value;

        // When
        var dto = labor.ToDto();

        // Then
        Assert.NotNull(dto);
        Assert.Equal(labor.Id, dto.LaborId);
        Assert.Equal(labor.FullName, dto.Name);
    }

    [Fact]
    public void ToDtos_Employees()
    {
        // Given
        var labor1 = EmployeeFactory.CreateLabor().Value;
        var labor2 = EmployeeFactory.CreateLabor().Value;

        var labors = new List<Employee>
        {
            labor1,
            labor2
        };

        // When
        var dtos = labors.ToDtos();

        // Then
        Assert.NotNull(dtos);
        Assert.Equal(2, dtos.Count);

        Assert.Equal(labor1.Id, dtos[0].LaborId);
        Assert.Equal(labor1.FullName, dtos[0].Name);

        Assert.Equal(labor2.Id, dtos[1].LaborId);
        Assert.Equal(labor2.FullName, dtos[1].Name);
    }
}

