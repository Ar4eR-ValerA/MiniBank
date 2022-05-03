using Microsoft.EntityFrameworkCore;
using MiniBank.Core.Domain.Users;
using MiniBank.Core.Domain.Users.Repositories;
using MiniBank.Core.Tools;
using MiniBank.Data.Contexts;

namespace MiniBank.Data.Users.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MiniBankContext _context;

    public UserRepository(MiniBankContext context)
    {
        _context = context;
    }

    public Task<bool> IsExist(Guid id, CancellationToken cancellationToken)
    {
        return _context.Users.AsNoTracking().AnyAsync(u => u.Id == id, cancellationToken);
    }

    public Task<bool> IsLoginExists(string login, CancellationToken cancellationToken)
    {
        return _context.Users.AnyAsync(u => u.Login == login, cancellationToken);
    }

    public async Task<User> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userDbModel = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (userDbModel is null)
        {
            throw new ObjectNotFoundException($"There is no user with such id: {id}");
        }

        return new User
        {
            Id = userDbModel.Id,
            Login = userDbModel.Login,
            Email = userDbModel.Email
        };
    }

    public async Task<IReadOnlyList<User>> GetAll(CancellationToken cancellationToken)
    {
        return await _context.Users.AsNoTracking().Select(u => new User
        {
            Id = u.Id,
            Login = u.Login,
            Email = u.Email
        }).ToListAsync(cancellationToken);
    }

    public async Task Create(User user, CancellationToken cancellationToken)
    {
        var userDbModel = new UserDbModel
        {
            Id = user.Id,
            Login = user.Login,
            Email = user.Email
        };

        await _context.Users.AddAsync(userDbModel, cancellationToken);
    }

    public async Task Update(User user, CancellationToken cancellationToken)
    {
        var userDbModel = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);

        if (userDbModel is null)
        {
            throw new ObjectNotFoundException($"There is no user with such id: {user.Id}");
        }

        userDbModel.Login = user.Login;
        userDbModel.Email = user.Email;

        _context.Users.Update(userDbModel);
    }

    public async Task Delete(Guid id, CancellationToken cancellationToken)
    {
        var userDbModel = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (userDbModel is null)
        {
            throw new ObjectNotFoundException($"There is no user with such id: {id}");
        }

        _context.Users.Remove(userDbModel);
    }
}