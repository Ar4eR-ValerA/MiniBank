using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MiniBank.Data.Users;

public class UserDbModel
{
    public Guid Id { get; set; }
    public string Login { get; set; }
    public string Email { get; set; }
    
    internal class Map : IEntityTypeConfiguration<UserDbModel>
    {
        public void Configure(EntityTypeBuilder<UserDbModel> builder)
        {
            builder.ToTable("user");
            builder.Property(u => u.Id).HasColumnName("id");
            builder.Property(u => u.Login).HasColumnName("login");
            builder.Property(u => u.Email).HasColumnName("Email");
        }
    }
}