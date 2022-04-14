namespace MiniBank.Core.Domain.Users.Services;

public interface IUserService
{
    Task<User> GetById(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetAll(CancellationToken cancellationToken = default);
    Task<Guid> Create(User user, CancellationToken cancellationToken = default);
    Task Update(User user, CancellationToken cancellationToken = default);
    Task Delete(Guid id, CancellationToken cancellationToken = default);
}