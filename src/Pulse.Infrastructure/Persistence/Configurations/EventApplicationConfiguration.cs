using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulse.Domain.Entities;

namespace Pulse.Infrastructure.Persistence.Configurations;

public class EventApplicationConfiguration : IEntityTypeConfiguration<EventApplication>
{
    public void Configure(EntityTypeBuilder<EventApplication> builder)
    {
        builder.HasKey(ea => ea.Id);

        builder.Property(ea => ea.Id)
            .ValueGeneratedNever();

        builder.HasIndex(ea => new { ea.EventId, ea.ApplicantId })
            .IsUnique();

        builder.HasIndex(ea => new { ea.EventId, ea.Status });

        builder.Property(ea => ea.Message)
            .HasMaxLength(500);

        builder.Property(ea => ea.Status)
            .IsRequired();

        builder.HasOne(ea => ea.Event)
            .WithMany(e => e.Applications)
            .HasForeignKey(ea => ea.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ea => ea.Applicant)
            .WithMany(u => u.Applications)
            .HasForeignKey(ea => ea.ApplicantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(ea => ea.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(ea => ea.UpdatedAt)
            .IsRequired()
            .HasColumnType("datetime2");
    }
}
