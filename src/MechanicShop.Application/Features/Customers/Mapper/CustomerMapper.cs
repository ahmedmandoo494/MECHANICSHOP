
using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Domain.Customers;


namespace  MechanicShop.Application.Features.Customers.Mapper;

public static class CustomerMapper
{
    public static CustomerDto ToDto(this Customer customer)
    {
        ArgumentNullException.ThrowIfNull(customer);
        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name!,
            PhoneNumber = customer.PhoneNumber!,
            Email = customer.Email!,
            Vehicles = customer.Vehicles
                .Select(v => v.ToDto())
                .ToList()
        };
    }
    public static List<CustomerDto> ToDtos(
        this IEnumerable<Customer> customers)
    {
        ArgumentNullException.ThrowIfNull(customers);
        return customers
            .Select(customer => customer.ToDto())
            .ToList();
    }
}
