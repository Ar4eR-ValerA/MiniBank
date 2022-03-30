using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MiniBank.Data.Accounts;
using MiniBank.Data.Transactions;
using MiniBank.Data.Users;

namespace MiniBank.Data.Contexts;

public class MiniBankContext : DbContext
{
    public DbSet<UserDbModel> Users { get; set; }
    public DbSet<AccountDbModel> Accounts { get; set; }
    public DbSet<TransactionDbModel> Transactions { get; set; }

    public MiniBankContext(DbContextOptions options)
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
        optionsBuilder.LogTo(Console.WriteLine);
        base.OnConfiguring(optionsBuilder);
    }
}

public class Factory : IDesignTimeDbContextFactory<MiniBankContext>
{
    public MiniBankContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder()
            .UseNpgsql("Host=localhost;Port=5432;Database=MiniBank1.0;Username=postgres;Password=123456")
            .Options;

        return new MiniBankContext(options);
    }
}