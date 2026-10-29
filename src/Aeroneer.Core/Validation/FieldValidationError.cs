namespace Aeroneer.Core.Validation;

/// <summary>Validation message associated with a registration field.</summary>
public sealed record FieldValidationError(string Field, string Message);
