using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Domain.Vehicles;


namespace  MechanicShop.Application.Features.Customers.Mapper;

public static class VehicleMapper
{
    public static VehicleDto ToDto(this Vehicle vehicle)
    {
        return new VehicleDto(vehicle.Id,vehicle.Make!,vehicle.Model!,vehicle.Year,vehicle.LicensePlate!);
    }
    public static List<VehicleDto> ToDtos(
        this IEnumerable<Vehicle> vehicles)
    {
        return vehicles
            .Select(vehicle => vehicle.ToDto())
            .ToList();
    }
}