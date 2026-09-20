namespace MechanicShop.Domain.Common.Results;

public readonly record struct Error
{
    public string Code { get; }
    public string Description { get; }
    public ErrorKind Type { get; }

    private Error(
        string code,
        string description,
        ErrorKind type)
    {
        Code = code;
        Description = description;
        Type = type;
    }

    public static Error Failure(
        string code = nameof(Failure),
        string description = "A general failure occurred.")
        => new(code, description, ErrorKind.Failure);

    public static Error Validation(
        string code = nameof(Validation),
        string description = "One or more validation errors occurred.")
        => new(code, description, ErrorKind.Validation);

    public static Error NotFound(
        string code = nameof(NotFound),
        string description = "The requested resource was not found.")
        => new(code, description, ErrorKind.NotFound);

    public static Error Conflict(
        string code = nameof(Conflict),
        string description = "A conflict occurred.")
        => new(code, description, ErrorKind.Conflict);

    public static Error Unauthorized(
        string code = nameof(Unauthorized),
        string description = "You are not authorized to perform this operation.")
        => new(code, description, ErrorKind.Unauthorized);

    public static Error Forbidden(
        string code = nameof(Forbidden),
        string description = "You do not have permission to perform this operation.")
        => new(code, description, ErrorKind.Forbidden);

    public static Error Unexpected(
        string code = nameof(Unexpected),
        string description = "An unexpected error occurred.")
        => new(code, description, ErrorKind.Unexpected);
}