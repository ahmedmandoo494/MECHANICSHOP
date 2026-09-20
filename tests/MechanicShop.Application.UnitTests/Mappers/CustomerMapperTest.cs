using MechanicShop.Application.Features.Customers.Mapper;
using MechanicShop.Domain.Customers;
using MechanicShop.Tests.Common.Customers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Xunit;

namespace MechanicShop.Application.UnitTests.Mappers;
public class CustomerMapperTests
{
    [Fact]
    public void ToDto_Customer()
    {
        // Given
        var name = "Ahmed";
        var phoneNumber = "01000000000";
        var email = "ahmed@test.com";

        var customer = CustomerFactory.CreateCustomer(
            name: name,
            phoneNumber: phoneNumber,
            email: email).Value;

        // When
        var dto = customer.ToDto();

        // Then
        Assert.NotNull(dto);
        Assert.Equal(customer.Id, dto.Id);
        Assert.Equal(name, dto.Name);
        Assert.Equal(phoneNumber, dto.PhoneNumber);
        Assert.Equal(email, dto.Email);
        Assert.NotNull(dto.Vehicles);
        Assert.Empty(dto.Vehicles);
    }

    [Fact]
    public void ToDto_Customer_WithVehicles()
    {
        // Given
        var name = "Ahmed";
        var phoneNumber = "01000000000";
        var email = "ahmed@test.com";

        var vehicle = VehicleFactory.CreateVehicle(
            make: "Honda",
            model: "Civic",
            year: 2023,
            licensePlate: "ABC 123").Value;

        var customer = CustomerFactory.CreateCustomer(
            name: name,
            phoneNumber: phoneNumber,
            email: email,
            vehicles: [vehicle]).Value;

        // When
        var dto = customer.ToDto();

        // Then
        Assert.NotNull(dto);
        Assert.Equal(customer.Id, dto.Id);
        Assert.Equal(name, dto.Name);
        Assert.Equal(phoneNumber, dto.PhoneNumber);
        Assert.Equal(email, dto.Email);

        Assert.NotNull(dto.Vehicles);
        Assert.Single(dto.Vehicles);

        var vehicleDto = dto.Vehicles[0];

        Assert.Equal(vehicle.Id, vehicleDto.VehicleId);
        Assert.Equal(vehicle.Make, vehicleDto.Make);
        Assert.Equal(vehicle.Model, vehicleDto.Model);
        Assert.Equal(vehicle.Year, vehicleDto.Year);
        Assert.Equal(vehicle.LicensePlate, vehicleDto.LicensePlate);
    }

    [Fact]
    public void ToDtos_Customers()
    {
        // Given
        var customer1 = CustomerFactory.CreateCustomer(
            name: "Ahmed",
            phoneNumber: "01000000000",
            email: "ahmed@test.com").Value;

        var customer2 = CustomerFactory.CreateCustomer(
            name: "Mohamed",
            phoneNumber: "01111111111",
            email: "mohamed@test.com").Value;

        var customers = new List<Customer>
        {
            customer1,
            customer2
        };

        // When
        var dtos = customers.ToDtos();

        // Then
        Assert.NotNull(dtos);
        Assert.Equal(2, dtos.Count);

        Assert.Equal(customer1.Id, dtos[0].Id);
        Assert.Equal(customer1.Name, dtos[0].Name);
        Assert.Equal(customer1.PhoneNumber, dtos[0].PhoneNumber);
        Assert.Equal(customer1.Email, dtos[0].Email);

        Assert.Equal(customer2.Id, dtos[1].Id);
        Assert.Equal(customer2.Name, dtos[1].Name);
        Assert.Equal(customer2.PhoneNumber, dtos[1].PhoneNumber);
        Assert.Equal(customer2.Email, dtos[1].Email);
    }
}