using Calculator.Domain.OperationsService;
using Calculator.Domain.Users.Errors;
using Calculator.Domain.Users.ValueObjects;
using FluentResults;

namespace Calculator.Domain.Users;

public class User
{

    public int Id { get; init; }
    public required string InternetAddress { get; init; }
    public ICollection<Operation> Operations { get; init; } = new List<Operation>();

    public static Result<User> Create(string ip)
    {
        if (string.IsNullOrWhiteSpace(ip))
            return Result.Fail(ErrorFactory.UserIdNotProvided());

        return new User 
        { 
            InternetAddress = ip
        };
    }

    public async Task<Result<Operation>> ComputeAndAppendOperation(Operation operation, IOperationService service, CancellationToken token = default)
    {

        Result<Operation> computation;
        if (!operation.IsCustom)
            computation = operation.Calculate();
        else
            computation = await operation.CustomCalculation(service, token);

        if (computation.IsFailed)
            return Result.Fail(computation.Errors);

        Operations.Add(computation.Value);

        return computation;
    }

    public Result AppendOperation(Operation? operation) 
    {

        if (operation is null)
            return Result.Fail(ErrorFactory.OperationProvidedIsNull());

        Operations.Add(operation);

        return Result.Ok();
    }

    public IEnumerable<Operation> YetComputedCalculations() =>
        Operations.Where(o => !o.WasComputed);

}
