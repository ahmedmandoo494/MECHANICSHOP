using MechanicShop.Application.Features.Billing.Mappers;
using MechanicShop.Domain.WorkOrders.Billing;
using MechanicShop.Tests.Common.Billing;
using MechanicShop.Tests.Common.Customers;
using MechanicShop.Tests.Common.Employees;
using MechanicShop.Tests.Common.RepaireTasks;
using MechanicShop.Tests.Common.WorkOrders;
using Xunit;

namespace MechanicShop.Application.UnitTests.Mappers;


public class InvoiceMapperTests
{
    [Fact]
    public void ToDto_ShouldMapCorrectly()
    {
        // Arrange
        var customer = CustomerFactory.CreateCustomer().Value;
        var vehicle = customer.Vehicles.First();

        var labor = EmployeeFactory.CreateLabor().Value;

        var repairTask = RepairTaskFactory.CreateRepairTask().Value;

        var workOrder = WorkOrderFactory.CreateWorkOrder(
            vehicleId: vehicle.Id,
            laborId: labor.Id,
            repairTasks: [repairTask]).Value;

        workOrder.Vehicle = vehicle;
        workOrder.Labor = labor;

        var invoiceLine = InvoiceLineItemFactory.CreateInvoiceLineItem(
            lineNumber: 1,
            description: "Oil Change",
            quantity: 2,
            unitPrice: 100m).Value;

        var invoice = InvoiceFactory.CreateInvoice(
            workOrderId: workOrder.Id,
            items: [invoiceLine]).Value;

        workOrder.Invoice = invoice;

        typeof(Invoice)
            .GetProperty(nameof(Invoice.WorkOrder))!
            .SetValue(invoice, workOrder);

        // Act
        var dto = invoice.ToDto();

        // Assert
        Assert.Equal(invoice.Id, dto.InvoiceId);
        Assert.Equal(invoice.WorkOrderId, dto.WorkOrderId);
        Assert.Equal(invoice.IssuedAtUtc, dto.IssuedAtUtc);

        Assert.Equal(invoice.SubTotal, dto.Subtotal);
        Assert.Equal(invoice.TaxAmount, dto.TaxAmount);
        Assert.Equal(invoice.DiscountAmount, dto.DiscountAmount);
        Assert.Equal(invoice.Total, dto.Total);

        Assert.Equal(invoice.Status.ToString(), dto.PaymentStatus);

        Assert.NotNull(dto.Customer);
        Assert.Equal(customer.Id, dto.Customer!.Id);
        Assert.Equal(customer.Name, dto.Customer.Name);
        Assert.Equal(customer.PhoneNumber, dto.Customer.PhoneNumber);
        Assert.Equal(customer.Email, dto.Customer.Email);

        Assert.NotNull(dto.Vehicle);
        Assert.Equal(vehicle.Id, dto.Vehicle!.VehicleId);
        Assert.Equal(vehicle.Make, dto.Vehicle.Make);
        Assert.Equal(vehicle.Model, dto.Vehicle.Model);
        Assert.Equal(vehicle.Year, dto.Vehicle.Year);
        Assert.Equal(vehicle.LicensePlate, dto.Vehicle.LicensePlate);

        Assert.Single(dto.Items);

        var itemDto = dto.Items[0];

        Assert.Equal(invoiceLine.InvoiceId, itemDto.InvoiceId);
        Assert.Equal(invoiceLine.LineNumber, itemDto.LineNumber);
        Assert.Equal(invoiceLine.Description, itemDto.Description);
        Assert.Equal(invoiceLine.Quantity, itemDto.Quantity);
        Assert.Equal(invoiceLine.UnitPrice, itemDto.UnitPrice);
        Assert.Equal(invoiceLine.LineTotal, itemDto.LineTotal);
    }

    [Fact]
    public void ToDtos_ShouldMapListCorrectly()
    {
        // Arrange
        var customer = CustomerFactory.CreateCustomer().Value;
        var vehicle = customer.Vehicles.First();

        var labor = EmployeeFactory.CreateLabor().Value;

        var workOrder = WorkOrderFactory.CreateWorkOrder(
            vehicleId: vehicle.Id,
            laborId: labor.Id).Value;

        workOrder.Vehicle = vehicle;
        workOrder.Labor = labor;

        var invoiceLine = InvoiceLineItemFactory.CreateInvoiceLineItem(
            lineNumber: 1,
            description: "Oil Change",
            quantity: 1,
            unitPrice: 100m).Value;

        var invoice = InvoiceFactory.CreateInvoice(
            workOrderId: workOrder.Id,
            items: [invoiceLine]).Value;

        workOrder.Invoice = invoice;

        typeof(Invoice)
            .GetProperty(nameof(Invoice.WorkOrder))!
            .SetValue(invoice, workOrder);

        var invoices = new List<Invoice>
        {
            invoice
        };

        // Act
        var dtos = invoices.ToDtos();

        // Assert
        Assert.Single(dtos);

        var dto = dtos[0];

        Assert.Equal(invoice.Id, dto.InvoiceId);
        Assert.Equal(invoice.WorkOrderId, dto.WorkOrderId);
        Assert.Equal(invoice.IssuedAtUtc, dto.IssuedAtUtc);

        Assert.Equal(invoice.SubTotal, dto.Subtotal);
        Assert.Equal(invoice.TaxAmount, dto.TaxAmount);
        Assert.Equal(invoice.DiscountAmount, dto.DiscountAmount);
        Assert.Equal(invoice.Total, dto.Total);

        Assert.Equal(invoice.Status.ToString(), dto.PaymentStatus);

        Assert.NotNull(dto.Customer);
        Assert.Equal(customer.Id, dto.Customer!.Id);

        Assert.NotNull(dto.Vehicle);
        Assert.Equal(vehicle.Id, dto.Vehicle!.VehicleId);

        Assert.Single(dto.Items);
        Assert.Equal(invoiceLine.Description, dto.Items[0].Description);
        Assert.Equal(invoiceLine.Quantity, dto.Items[0].Quantity);
        Assert.Equal(invoiceLine.UnitPrice, dto.Items[0].UnitPrice);
        Assert.Equal(invoiceLine.LineTotal, dto.Items[0].LineTotal);
    }
}
