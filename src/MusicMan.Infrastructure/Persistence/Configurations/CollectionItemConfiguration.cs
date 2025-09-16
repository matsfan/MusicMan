using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicMan.Domain.Entities;

namespace MusicMan.Infrastructure.Persistence.Configurations;

public class CollectionItemConfiguration : IEntityTypeConfiguration<CollectionItem>
{
    public void Configure(EntityTypeBuilder<CollectionItem> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Notes).HasMaxLength(1024);
        builder.Property(x => x.AddedAtUtc).IsRequired();

        builder.HasOne(x => x.Album)
            .WithMany()
            .HasForeignKey(x => x.AlbumId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.AlbumId);
    }
}
