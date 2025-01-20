
using Calculator.Domain.OperationsService;
using Calculator.Domain.Users.Errors;
using FluentResults;

namespace Calculator.Domain.Users.ValueObjects;

public class Operation : ValueObject
{

    public required string Value1 { get; init; }
    public required string Value2 { get; init; }
    public required string? Result { get; init; }
    public required Computation Computation { get; init; }
    public required string? Custom { get; init; }
    public DateTime CreatedInUtc { get; init; } = DateTime.UtcNow;

    public bool WasComputed => !string.IsNullOrEmpty(Result);
    public bool IsCustom => Computation.Custom == Computation;

    private Operation() { }

    public static Result<Operation> Create(string value1, string value2, int computationValue, string? custom)
    {
        if (string.IsNullOrWhiteSpace(value1) || string.IsNullOrWhiteSpace(value2))
            return ErrorFactory.OneOfTheProvidedValuesAreNullOrWhiteSpace();

        if (!Computation.TryFromValue(computationValue, out var computation))
            return ErrorFactory.OperationProvidedIsInvalid();

        if (computation.Name == nameof(Computation.Custom) &&
            string.IsNullOrWhiteSpace(custom))
            return ErrorFactory.CustomOperationsCannotBeInvokedWithDefaultOperations();

        return new Operation
        {
            Computation = computation,
            Value1 = value1,
            Value2 = value2,
            Result = null,
            Custom = custom,
        };
    }

    public Result<Operation> Calculate() => Computation.Compute(this);

    public async Task<Result<Operation>> CustomCalculation(IOperationService? service = null, CancellationToken token = default)
    {
        if (service is null)
            return ErrorFactory.CustomCalculationsServiceNotProvided();

        return await service.GenerateResult(this, token);
    }

    internal Operation SetResult(string? result)
    {
        return new Operation
        {
            Value1 = Value1,
            Value2 = Value2,
            Computation = Computation,
            Custom = Custom,
            Result = result,
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value1;
        yield return Value2;
        yield return Computation;
    }
}
