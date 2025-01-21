
using FluentResults;

namespace Calculator.Persistence.Context;

public interface IUnitOfWork
{
    public Task<Result> Commit(CancellationToken token = default);
    public Task<Result> Dispose();
}
