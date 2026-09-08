using CleanArchitect.Domain.UserAggregate;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitect.Infrastructure.UserAggregate;

public sealed class UserEntityConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.DisplayName).HasMaxLength(256);

        builder.HasOne<AppUser>()
            .WithOne()
            .HasForeignKey<User>(e => e.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
