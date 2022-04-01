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

    public Task<bool> IsExist(Guid id)
    {
        return _context.Users.AsNoTracking().AnyAsync(u => u.Id == id);
    }

    public Task<bool> IsLoginExists(string login)
    {
        return _context.Users.AnyAsync(u => u.Login == login);
    }

    public async Task<User> GetById(Guid id)
    {
        var userDbModel = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

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

    public async Task<IEnumerable<User>> GetAll()
    {
        return await _context.Users.AsNoTracking().Select(u => new User
        {
            Id = u.Id,
            Login = u.Login,
            Email = u.Email
        }).ToListAsync();
    }

    public async Task Create(User user)
    {
        var userDbModel = new UserDbModel
        {
            Id = user.Id,
            Login = user.Login,
            Email = user.Email
        };

        await _context.Users.AddAsync(userDbModel);
    }

    public async Task Update(User user)
    {
        var userDbModel = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);

        if (userDbModel is null)
        {
            throw new ObjectNotFoundException($"There is no user with such id: {user.Id}");
        }

        userDbModel.Login = user.Login;
        userDbModel.Email = user.Email;

        _context.Users.Update(userDbModel);
    }

    public async Task Delete(Guid id)
    {
        var userDbModel = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (userDbModel is null)
        {
            throw new ObjectNotFoundException($"There is no user with such id: {id}");
        }

        _context.Users.Remove(userDbModel);
    }
}