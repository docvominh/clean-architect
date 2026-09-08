using CleanArchitect.Domain.OrderAggregate;
using CleanArchitect.Domain.ProductAggregate;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitect.Infrastructure.OrderAggregate;

public sealed class OrderProductEntityConfig : IEntityTypeConfiguration<OrderProduct>
{
    public void Configure(EntityTypeBuilder<OrderProduct> builder)
    {
        builder.ToTable("OrderProducts");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Quantity).IsRequired();
        builder.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(e => e.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.OrderId, e.ProductId });
    }
}
