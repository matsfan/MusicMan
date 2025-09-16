using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicMan.Domain.Entities;

namespace MusicMan.Infrastructure.Persistence.Configurations;

public class AlbumConfiguration : IEntityTypeConfiguration<Album>
{
    public void Configure(EntityTypeBuilder<Album> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(512).IsRequired();
        builder.Property(x => x.Artist).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Country).HasMaxLength(64);
        builder.Property(x => x.CoverImageUrl).HasMaxLength(1024);
        builder.Property(x => x.Barcode).HasMaxLength(64);

        builder.HasIndex(x => x.DiscogsReleaseId).IsUnique();
        builder.HasIndex(x => x.Barcode);
    }
}
