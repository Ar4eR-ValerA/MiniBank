namespace MiniBank.Core.Domain.Users.Repositories;

public interface IUserRepository
{
    Task<bool> IsExist(Guid id, CancellationToken cancellationToken);
    Task<bool> IsLoginExists(string login, CancellationToken cancellationToken);
    Task<User> GetById(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<User>> GetAll(CancellationToken cancellationToken);
    Task Create(User user, CancellationToken cancellationToken);
    Task Update(User user, CancellationToken cancellationToken);
    Task Delete(Guid id, CancellationToken cancellationToken);
}