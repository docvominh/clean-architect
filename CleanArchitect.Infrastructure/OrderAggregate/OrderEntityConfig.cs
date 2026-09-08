using CleanArchitect.Domain.OrderAggregate;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitect.Infrastructure.OrderAggregate;

public class OrderEntityConfig : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(e => e.Id);

        builder.Property(a => a.Country).HasColumnName("ShippingCountry").HasMaxLength(100).IsRequired();
        builder.Property(a => a.State).HasColumnName("ShippingState").HasMaxLength(100);
        builder.Property(a => a.City).HasColumnName("ShippingCity").HasMaxLength(100).IsRequired();
        builder.Property(a => a.Street).HasColumnName("ShippingStreet").HasMaxLength(256).IsRequired();
        builder.Property(a => a.ContactPhoneNumber).HasColumnName("ShippingContactPhoneNumber").HasMaxLength(32).IsRequired();

        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(32);
        builder.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");

        builder.HasMany(e => e.OrderProducts)
            .WithOne()
            .HasForeignKey(e => e.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
