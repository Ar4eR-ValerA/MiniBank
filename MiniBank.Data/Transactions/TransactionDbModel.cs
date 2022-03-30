using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniBank.Core.Domain.Currencies;

namespace MiniBank.Data.Transactions;

public class TransactionDbModel
{
    public Guid Id { get; set; }
    public double Amount { get; set; }
    public double Commission { get; set; }
    public Currency Currency { get; set; }
    public Guid FromAccountId { get; set; }
    public Guid ToAccountId { get; set; }
    
    internal class Map : IEntityTypeConfiguration<TransactionDbModel>
    {
        public void Configure(EntityTypeBuilder<TransactionDbModel> builder)
        {
            builder.ToTable("transaction");
            builder.Property(t => t.Id).HasColumnName("id");
            builder.Property(t => t.Amount).HasColumnName("amount");
            builder.Property(t => t.Commission).HasColumnName("commission");
            builder.Property(t => t.Currency).HasColumnName("currency");
            builder.Property(t => t.FromAccountId).HasColumnName("from_account_id");
            builder.Property(t => t.ToAccountId).HasColumnName("to_account_id");
        }
    }
}