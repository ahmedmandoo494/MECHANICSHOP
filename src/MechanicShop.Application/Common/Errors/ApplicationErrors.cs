using MechanicShop.Domain.Common.Results;

namespace  MechanicShop.Application.Common.Errors;

public static class ApplicationErrors
{
    public static Error WorkOrderOutsideOperatingHour(
        DateTimeOffset startAtUtc,
        DateTimeOffset endAtUtc) =>
        Error.Conflict(
            "ApplicationErrors.WorkOrder.OutsideOperatingHour",
            $"The WorkOrder time ({startAtUtc} - {endAtUtc}) is outside of store operating hours.");

    public static readonly Error WorkOrderNotFound =
        Error.NotFound(
            "ApplicationErrors.WorkOrder.NotFound",
            "WorkOrder does not exist.");

    public static readonly Error LaborOccupied =
        Error.Conflict(
            "ApplicationErrors.Labor.Occupied",
            "Labor is already occupied during the requested time.");

    public static readonly Error CustomerNotFound =
        Error.NotFound(
            "ApplicationErrors.Customer.NotFound",
            "Customer does not exist.");

    public static readonly Error VehicleNotFound =
        Error.NotFound(
            "ApplicationErrors.Vehicle.NotFound",
            "Vehicle does not exist.");

    public static readonly Error VehicleSchedulingConflict =
        Error.Conflict(
            "ApplicationErrors.Vehicle.SchedulingConflict",
            "The vehicle already has an overlapping WorkOrder.");

    public static readonly Error RepairTaskNotFound =
        Error.NotFound(
            "ApplicationErrors.RepairTask.NotFound",
            "Repair task does not exist.");

    public static readonly Error WorkOrderMustBeCompletedForInvoicing =
        Error.Conflict(
            "ApplicationErrors.WorkOrder.InvoiceIssuance.InvalidState",
            "WorkOrder must be in 'Completed' state to issue an invoice.");

    public static readonly Error InvoiceNotFound =
        Error.NotFound(
            "ApplicationErrors.Invoice.NotFound",
            "Invoice does not exist.");

    public static readonly Error InvalidRefreshToken =
        Error.Validation(
            "ApplicationErrors.RefreshToken.Expiry.Invalid",
            "Expiry must be in the future.");

    public static readonly Error ExpiredAccessTokenInvalid =
        Error.Conflict(
            "ApplicationErrors.Auth.ExpiredAccessToken.Invalid",
            "Expired access token is not valid.");

    public static readonly Error UserIdClaimInvalid =
        Error.Conflict(
            "ApplicationErrors.Auth.UserIdClaim.Invalid",
            "Invalid userId claim.");

    public static readonly Error RefreshTokenExpired =
        Error.Conflict(
            "ApplicationErrors.Auth.RefreshToken.Expired",
            "Refresh token is invalid or has expired.");

    public static readonly Error UserNotFound =
        Error.NotFound(
            "ApplicationErrors.Auth.User.NotFound",
            "User not found.");

    public static readonly Error TokenGenerationFailed =
        Error.Failure(
            "ApplicationErrors.Auth.TokenGeneration.Failed",
            "Failed to generate new JWT token.");

    public static readonly Error LaborNotFound =
        Error.NotFound(
            "ApplicationErrors.Labor.NotFound",
            "Labor does not exist.");
    public static readonly Error InvoiceAlreadyExists =
        Error.Conflict(
            "ApplicationErrors.Invoice.AlreadyExists",
            "An invoice already exists for this WorkOrder.");
}