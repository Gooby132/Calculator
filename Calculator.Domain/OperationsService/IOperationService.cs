using Calculator.Domain.Users.ValueObjects;
using FluentResults;

namespace Calculator.Domain.OperationsService;

public interface IOperationService
{
    public Task<Result<Operation>> GenerateResult(Operation operation, CancellationToken token = default);
}
