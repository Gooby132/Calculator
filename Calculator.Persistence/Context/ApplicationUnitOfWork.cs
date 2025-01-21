using FluentResults;

namespace Calculator.Persistence.Context;

public class ApplicationUnitOfWork : IUnitOfWork
{
    private readonly ApplicationContext _context;

    public ApplicationUnitOfWork(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<Result> Commit(CancellationToken token = default)
    {
        try
        {
            await _context.SaveChangesAsync(token);

            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Fail(new Error("db error").CausedBy(e));
        }
    }

    public async Task<Result> Dispose()
    {
        try
        {
            await _context.DisposeAsync();

            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Fail(new Error("db error").CausedBy(e));
        }
    }
}
