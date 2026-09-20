using Asp.Versioning;
using MechanicShop.Application.Features.Billing.Commands.IssueInvoice;
using MechanicShop.Application.Features.Billing.Commands.SettleInvoice;
using MechanicShop.Application.Features.Billing.Dtos;
using MechanicShop.Application.Features.Billing.Queries.GetInvoiceById;
using MechanicShop.Application.Features.Billing.Queries.GetInvoicePdf;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace MechanicShop.Api.Controllers;

[Route("api/v{version:ApiVersion}/invoices")]
[ApiVersion("1.0")]
[Authorize]
public sealed class InvoicesController(ISender sender) : ApiController
{
    [HttpPost("Workorders/{workOrderId:guid}")]
    [Authorize(Policy = "ManagerOnly")]
    [ProducesResponseType(typeof(InvoiceDto),StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Issues an invoice for a work order.")]
    [EndpointDescription("Create a new invoice for the specified work order and returns the created invoice resource.")]
    [EndpointName("IssueInvoiceForWorkOrder")]
    [MapToApiVersion("1.0")]

    public async Task<IActionResult> IssueInvoice(Guid workOrderId,CancellationToken ct)
    {
        var result = await sender.Send(new IssueInvoiceCommand(workOrderId),ct);
        return result.Match(
            response => CreatedAtAction(
                nameof(GetInvoice),
                new
                {
                    version = "1.0",
                    invoiceId = response.InvoiceId
                },
                response),
                    Problem
                );
    }

    [HttpGet("{invoiceId:guid}")]
    [Authorize(Policy = "ManagerOnly")]
    [ProducesResponseType(typeof(InvoiceDto),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves an invoice by id.")]
    [EndpointDescription("Returns detailed information about the specified invoice, Only users with the manager role ate authorized.")]
    [EndpointName("GetInvoice")]
    [MapToApiVersion("1.0")]

    public async Task<IActionResult> GetInvoice(Guid invoiceId,CancellationToken ct)
    {
        var result = await sender.Send(new GetInvoiceByIdQuery(invoiceId),ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("{invoiceId:guid}/pdf")]
    [Authorize(Policy = "ManagerOnly")]
    [ProducesResponseType(typeof(InvoiceDto),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Download the invoice as a PDF file.")]
    [EndpointDescription("Returns the invoice PDF file for the specified invoice ID, Only users with the manager role ate authorized.")]
    [EndpointName("GetInvoicePdf")]
    [MapToApiVersion("1.0")]

    public async Task<IActionResult> GetInvoicePdf(Guid invoiceId,CancellationToken ct)
    {
        var result = await sender.Send(new GetInvoicePdfQuery(invoiceId),ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpPut("{invoiceId:guid}/payments")]
    [Authorize(Policy = "ManagerOnly")]
    [ProducesResponseType(typeof(InvoiceDto),StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Mark as invoice as paid.")]
    [EndpointDescription("Settles the specified invoice, Only users with the manager role are authorized to perform this operaion.")]
    [EndpointName("SettleInvoice")]
    [MapToApiVersion("1.0")]

    public async Task<IActionResult> SettleInvoice(Guid invoiceId,CancellationToken ct)
    {
        var result = await sender.Send(new SettleInvoiceCommand(invoiceId),ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

}
