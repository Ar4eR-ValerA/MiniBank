namespace MiniBank.Core;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}