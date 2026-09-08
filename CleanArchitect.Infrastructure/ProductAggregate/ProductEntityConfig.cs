using CleanArchitect.Domain.Product;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitect.Infrastructure.ProductAggregate;

public sealed class ProductEntityConfig : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).HasMaxLength(256).IsRequired();
        builder.Property(e => e.Manufacturer).HasMaxLength(256).IsRequired();
        builder.Property(e => e.ImageUrl).HasMaxLength(2048);
        builder.Property(e => e.Price).HasColumnType("decimal(18,2)");
        builder.Property(e => e.PriceDiscount).HasColumnType("decimal(18,2)");
    }
}
