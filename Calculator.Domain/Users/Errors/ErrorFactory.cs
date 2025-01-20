using Calculator.Domain.Shared;
using Calculator.Domain.Users.ValueObjects;
using FluentResults;

namespace Calculator.Domain.Users.Errors;

public static class ErrorFactory
{
    public static ErrorBase OneOrMoreOfValuesProvidedAreNotNumbers() =>
        new ErrorBase(1, "One or more of values provided are not numbers");

    public static ErrorBase CustomOperationsCannotBeInvokedWithDefaultOperations() =>
        new ErrorBase(2, "Custom operations cannot be invoked with default operations");

    public static ErrorBase UndefinedOperationBehavior() =>
        new ErrorBase(3, "Undefined operation behavior");

    public static ErrorBase OneOfTheProvidedValuesAreNullOrWhiteSpace() =>
        new ErrorBase(4, "One of the provided values are null or whiteSpace");

    public static ErrorBase OperationEncounteredDivisionByZeroError() =>
        new ErrorBase(5, "Operation encountered division by zero error");

    public static ErrorBase FailedAppendingOperation(Exception e)
    {
        var error = new ErrorBase(6, $"Failed appending operation. error - {e.Message}");
        error.CausedBy(e);
        return error;
    }

    public static ErrorBase CustomCalculationsServiceNotProvided() =>
        new ErrorBase(7, "custom calculation service not provided");

    public static ErrorBase OperationProvidedIsNull() =>
        new ErrorBase(8, "operation not provided");

    public static Result<Operation> OperationProvidedIsInvalid() =>
        new ErrorBase(9, "operation provided is invalid");
}
