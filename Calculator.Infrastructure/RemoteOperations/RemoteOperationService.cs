using Calculator.Domain.OperationsService;
using Calculator.Domain.Users.ValueObjects;
using FluentResults;

namespace Calculator.Infrastructure.RemoteOperations;

public class RemoteOperationService : IOperationService
{
    public Task<Result<Operation>> GenerateResult(Operation operation, CancellationToken token = default)
    {
        return Task.FromResult(Result.Fail<Operation>("not implemented"));
    }

    public Task<Result<IEnumerable<Operation>>> GetAvailableOperations(CancellationToken token = default)
    {
        return Task.FromResult(Result.Fail<IEnumerable<Operation>>("not implemented"));
    }
}
