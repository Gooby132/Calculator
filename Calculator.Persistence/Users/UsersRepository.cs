using Calculator.Domain.Users;
using Calculator.Persistence.Context;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Calculator.Persistence.Users;

public class UsersRepository : IUserRepository
{
    private readonly ApplicationContext _context;

    public UsersRepository(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<Result<IEnumerable<User>>> GetAll(CancellationToken token = default)
    {
        try
        {
            return await _context.Users.ToArrayAsync(token);
        }
        catch (Exception e)
        {
            return Result.Fail(new Error("db error").CausedBy(e));
        }
    }


    public async Task<Result<User?>> GetById(int id, CancellationToken token = default)
    {
        try
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id, token);
        }
        catch (Exception e)
        {
            return Result.Fail(new Error("db error").CausedBy(e));
        }
    }

    public async Task<Result<User?>> GetByInternetAddress(string internetAddress, CancellationToken token = default)
    {
        try
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.InternetAddress == internetAddress, token);
        }
        catch (Exception e)
        {
            return Result.Fail(new Error("db error").CausedBy(e));
        }
    }

    public async Task<Result<User?>> GetOperations(int id, int size, int page, string? fromInUtc = null, string? untilInUtc = null, CancellationToken token = default)
    {
        try
        {
            return await _context.Users
                .Include(u => u.Operations)
                .Skip(page)
                .Take(page)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id, token);
        }
        catch (Exception e)
        {
            return Result.Fail(new Error("db error").CausedBy(e));
        }
    }

    public async Task<Result> Persist(User user, CancellationToken token = default)
    {
        try
        {
            await _context.Users.AddAsync(user, token);
            return Result.Ok();
        }
        catch (Exception)
        {
            return Result.Fail("db error");
        }
    }

    public Result<User> Update(User user)
    {
        try
        {
            return _context.Users.Update(user).Entity;
        }
        catch (Exception)
        {
            return Result.Fail("db error");
        }
    }
}
