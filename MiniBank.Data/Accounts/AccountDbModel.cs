using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniBank.Core.Domain.Currencies;

namespace MiniBank.Data.Accounts;

public class AccountDbModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public double Balance { get; set; }
    public Currency Currency { get; set; }
    public bool IsActive { get; set; }
    public DateTime DateOpened { get; set; }
    public DateTime? DateClosed { get; set; }

    internal class Map : IEntityTypeConfiguration<AccountDbModel>
    {
        public void Configure(EntityTypeBuilder<AccountDbModel> builder)
        {
            builder.ToTable("account");
            builder.Property(a => a.Id).HasColumnName("id");
            builder.Property(a => a.UserId).HasColumnName("user_id");
            builder.Property(a => a.Balance).HasColumnName("balance");
            builder.Property(a => a.Currency).HasColumnName("currency");
            builder.Property(a => a.IsActive).HasColumnName("is_active");
            builder.Property(a => a.DateOpened).HasColumnName("date_opened");
            builder.Property(a => a.DateClosed).HasColumnName("date_closed");
        }
    }
}