using Calculator.Domain.OperationsService;
using Calculator.Domain.Users.Errors;
using Calculator.Domain.Users.ValueObjects;
using FluentResults;

namespace Calculator.Domain.Users;

public class User
{

    public required string Id { get; init; }
    public ICollection<Operation> Operations { get; init; } = new List<Operation>();

    public async Task<Result> ComputeAndAppendOperation(Operation operation, IOperationService service, CancellationToken token = default)
    {

        Result<Operation> computation;
        if (operation.IsCustom)
            computation = operation.Calculate();
        else
            computation = await operation.CustomCalculation(service, token);

        if (computation.IsFailed)
            return Result.Fail(computation.Errors);

        Operations.Add(computation.Value);

        return Result.Ok();
    }

    public Result AppendOperation(Operation? operation) 
    {
        Operations.Add(operation);

    }

    public IEnumerable<Operation> YetComputedCalculations() =>
        Operations.Where(o => !o.WasComputed);

}
