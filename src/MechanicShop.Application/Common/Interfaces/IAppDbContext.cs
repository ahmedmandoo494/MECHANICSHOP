using MechanicShop.Application.Features.Customers.Queries.GetCustomers;
using MechanicShop.Domain.Customers;
using MechanicShop.Domain.Employees;
using MechanicShop.Domain.Identity;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Domain.RepairTasks.Parts;
using MechanicShop.Domain.Vehicles;
using MechanicShop.Domain.WorkOrders;
using MechanicShop.Domain.WorkOrders.Billing;
using Microsoft.EntityFrameworkCore;

namespace MechanicShop.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Customer> Customers{get;}
    DbSet<Vehicle> Vehicles{get;}
    DbSet<WorkOrder> WorkOrders{get;}
    DbSet<RepairTask> RepairTasks{get;}
    DbSet<Part> Parts{get;}
    DbSet<RefreshToken> RefreshTokens{get;}
    DbSet<Invoice> Invoices{get;}
    DbSet<Employee> Employees{get;}






    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

}