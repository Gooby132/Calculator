using FluentResults;

namespace Calculator.Domain.Users;

public interface IUserRepository
{

    public Task<Result> Persist(User user, CancellationToken token = default);
    
    public Task<Result<User>> Update(User user, CancellationToken token = default);

    public Task<Result<User?>> GetById(string id, CancellationToken token = default);

    public Task<Result<IEnumerable<User>>> GetAll(CancellationToken token = default);

}
