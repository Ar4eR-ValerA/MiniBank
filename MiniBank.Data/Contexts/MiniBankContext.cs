using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiniBank.Data.Accounts;
using MiniBank.Data.Transactions;
using MiniBank.Data.Users;

namespace MiniBank.Data.Contexts;

public class MiniBankContext : DbContext
{
    public DbSet<UserDbModel> Users { get; set; }
    public DbSet<AccountDbModel> Accounts { get; set; }
    public DbSet<TransactionDbModel> Transactions { get; set; }

    public MiniBankContext(DbContextOptions<MiniBankContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MiniBankContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
        base.OnConfiguring(optionsBuilder);
    }
}