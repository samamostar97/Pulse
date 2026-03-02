using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulse.Domain.Entities;

namespace Pulse.Infrastructure.Persistence.Configurations;

public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.HasKey(cm => cm.Id);

        builder.Property(cm => cm.Id)
            .ValueGeneratedNever();

        builder.Property(cm => cm.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(cm => cm.SentAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.HasIndex(cm => new { cm.EventId, cm.SentAt });

        builder.HasOne(cm => cm.Event)
            .WithMany(e => e.ChatMessages)
            .HasForeignKey(cm => cm.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cm => cm.Sender)
            .WithMany(u => u.ChatMessages)
            .HasForeignKey(cm => cm.SenderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
