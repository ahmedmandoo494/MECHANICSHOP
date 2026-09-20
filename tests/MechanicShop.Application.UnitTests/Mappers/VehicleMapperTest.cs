using MechanicShop.Application.Features.Customers.Mapper;
using MechanicShop.Domain.Vehicles;
using MechanicShop.Tests.Common.Customers;
using Xunit;

namespace MechanicShop.Application.UnitTests.Mappers;

public class VehicleMapperTest
{
    [Fact]
    public void ToDto_Vehicle()
    {
        // Given
        var make = "honda";
        var model = "abc";
        var year = 2023;
        var licensePlate = "afc 453";

        var vehicle = VehicleFactory.CreateVehicle(make:make,model:model,year:year,licensePlate:licensePlate).Value;
        // When
        var dto = vehicle.ToDto();
        // Then
        Assert.Equal(vehicle.Id,dto.VehicleId);
        Assert.Equal(make,dto.Make);
        Assert.Equal(model,dto.Model);
        Assert.Equal(year,dto.Year);
        Assert.Equal(licensePlate,dto.LicensePlate);
    }

    [Fact]
    public void ToDtos_Vehicles()
    {
        // Given
        var make = "honda";
        var model = "abc";
        var year = 2023;
        var licensePlate = "afc 453";

        var vehicle = VehicleFactory.CreateVehicle(make:make,model:model,year:year,licensePlate:licensePlate).Value;

        var vehicles = new List<Vehicle>{vehicle};
        // When
        var dtos = vehicles.ToDtos();
        // Then
        Assert.Single(dtos);
        var dto = dtos[0];
        Assert.Equal(vehicle.Id,dto.VehicleId);
        Assert.Equal(make,dto.Make);
        Assert.Equal(model,dto.Model);
        Assert.Equal(year,dto.Year);
        Assert.Equal(licensePlate,dto.LicensePlate);
    }

   
}
