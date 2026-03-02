using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulse.Domain.Entities;

namespace Pulse.Infrastructure.Persistence.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.OwnsOne(e => e.Location, loc =>
        {
            loc.Property(l => l.Latitude).HasColumnName("Latitude").IsRequired();
            loc.Property(l => l.Longitude).HasColumnName("Longitude").IsRequired();

            loc.HasIndex(l => new { l.Latitude, l.Longitude });
        });

        builder.Property(e => e.Address)
            .HasMaxLength(500);

        builder.Property(e => e.StartsAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.HasIndex(e => e.StartsAt);

        builder.Property(e => e.EndsAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(e => e.MaxCapacity)
            .IsRequired();

        builder.Property(e => e.CategoryType)
            .IsRequired();

        builder.HasIndex(e => e.CategoryType);

        builder.Property(e => e.Status)
            .IsRequired();

        builder.Property(e => e.Visibility)
            .IsRequired();

        builder.Property(e => e.AutoApprove)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.ImageUrl)
            .HasMaxLength(500);

        builder.HasIndex(e => e.OrganizerId);

        builder.HasOne(e => e.Organizer)
            .WithMany(u => u.OrganizedEvents)
            .HasForeignKey(e => e.OrganizerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasColumnType("datetime2");
    }
}
