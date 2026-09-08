using CleanArchitect.Domain.UserAggregate;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitect.Infrastructure.UserAggregate;

public sealed class UserAddressEntityConfig : IEntityTypeConfiguration<UserAddress>
{
    public void Configure(EntityTypeBuilder<UserAddress> builder)
    {
        builder.ToTable("UserAddresses");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.UserId).IsRequired();

        builder.Property(a => a.Country).HasColumnName("Country").HasMaxLength(100).IsRequired();
        builder.Property(a => a.State).HasColumnName("State").HasMaxLength(100);
        builder.Property(a => a.City).HasColumnName("City").HasMaxLength(100).IsRequired();
        builder.Property(a => a.Street).HasColumnName("Street").HasMaxLength(256).IsRequired();
        builder.Property(a => a.ContactPhoneNumber).HasColumnName("ContactPhoneNumber").HasMaxLength(32).IsRequired();

        builder.HasOne<User>()
            .WithMany(u => u.Addresses)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.UserId);

        builder.HasIndex(e => e.UserId)
            .IsUnique()
            .HasFilter("[IsDefault] = 1")
            .HasDatabaseName("IX_UserAddresses_UserId_IsDefault");
    }
}
