using MiniBank.Core;

namespace MiniBank.Data.Contexts;

public class EfUnitOfWork : IUnitOfWork
{
    private readonly MiniBankContext _context;

    public EfUnitOfWork(MiniBankContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}