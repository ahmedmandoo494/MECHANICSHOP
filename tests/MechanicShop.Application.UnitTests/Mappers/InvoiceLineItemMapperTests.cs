using MechanicShop.Application.Features.Billing.Mappers;
using MechanicShop.Domain.WorkOrders.Billing;
using MechanicShop.Tests.Common.Billing;
using Xunit;

namespace MechanicShop.Application.UnitTests.Mappers;

public class InvoiceLineItemMapperTests
{
    [Fact]
    public void ToDto_InvoiceLineItem()
    {
        // Given
        var item = InvoiceLineItemFactory.CreateInvoiceLineItem(
            lineNumber: 1,
            description: "Oil Change",
            quantity: 2,
            unitPrice: 50m).Value;

        // When
        var dto = item.ToDto();

        // Then
        Assert.NotNull(dto);
        Assert.Equal(item.InvoiceId, dto.InvoiceId);
        Assert.Equal(item.LineNumber, dto.LineNumber);
        Assert.Equal(item.Description, dto.Description);
        Assert.Equal(item.Quantity, dto.Quantity);
        Assert.Equal(item.UnitPrice, dto.UnitPrice);
        Assert.Equal(item.LineTotal, dto.LineTotal);
    }

    [Fact]
    public void ToDtos_InvoiceLineItems()
    {
        // Given
        var item1 = InvoiceLineItemFactory.CreateInvoiceLineItem(
            lineNumber: 1,
            description: "Oil Change",
            quantity: 2,
            unitPrice: 50m).Value;

        var item2 = InvoiceLineItemFactory.CreateInvoiceLineItem(
            lineNumber: 2,
            description: "Brake Repair",
            quantity: 1,
            unitPrice: 100m).Value;

        var items = new List<InvoiceLineItem>
        {
            item1,
            item2
        };

        // When
        var dtos = items.ToDtos();

        // Then
        Assert.NotNull(dtos);
        Assert.Equal(2, dtos.Count);

        Assert.Equal(item1.InvoiceId, dtos[0].InvoiceId);
        Assert.Equal(item1.LineNumber, dtos[0].LineNumber);
        Assert.Equal(item1.Description, dtos[0].Description);
        Assert.Equal(item1.Quantity, dtos[0].Quantity);
        Assert.Equal(item1.UnitPrice, dtos[0].UnitPrice);
        Assert.Equal(item1.LineTotal, dtos[0].LineTotal);

        Assert.Equal(item2.InvoiceId, dtos[1].InvoiceId);
        Assert.Equal(item2.LineNumber, dtos[1].LineNumber);
        Assert.Equal(item2.Description, dtos[1].Description);
        Assert.Equal(item2.Quantity, dtos[1].Quantity);
        Assert.Equal(item2.UnitPrice, dtos[1].UnitPrice);
        Assert.Equal(item2.LineTotal, dtos[1].LineTotal);
    }
}