using FluentResults;

namespace Calculator.Domain.Users;

public interface IUserRepository
{

    public Task<Result> Persist(User user, CancellationToken token = default);
    
    public Result<User> Update(User user);

    public Task<Result<User?>> GetById(int id, CancellationToken token = default);

    public Task<Result<User?>> GetByInternetAddress(string internetAddress, CancellationToken token = default);

    public Task<Result<IEnumerable<User>>> GetAll(CancellationToken token = default);

    public Task<Result<User?>> GetOperations(int id, int size, int page, string? fromInUtc = null, string? untilInUtc = null, CancellationToken token = default);

}
