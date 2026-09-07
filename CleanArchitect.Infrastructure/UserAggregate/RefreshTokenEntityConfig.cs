using CleanArchitect.Domain.UserAggregate;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitect.Infrastructure.UserAggregate;

public sealed class RefreshTokenEntityConfig : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("Id");

        builder.Property(e => e.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(e => e.Token).HasColumnName("Token").HasMaxLength(512).IsRequired();
        builder.HasIndex(e => e.Token).IsUnique();
        builder.Property(e => e.ExpiresAt).HasColumnName("ExpiresAt");
        builder.Property(e => e.RevokedAt).HasColumnName("RevokedAt");
        builder.Property(e => e.ReplacedByToken).HasColumnName("ReplacedByToken").HasMaxLength(512);
        builder.Property(e => e.CreatedBy).HasColumnName("CreatedBy").HasMaxLength(50);
        builder.Property(e => e.CreatedOn).HasColumnName("CreatedAt");
        builder.Property(e => e.ModifiedBy).HasColumnName("ModifiedBy").HasMaxLength(50);
        builder.Property(e => e.ModifiedOn).HasColumnName("ModifiedAt");

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}