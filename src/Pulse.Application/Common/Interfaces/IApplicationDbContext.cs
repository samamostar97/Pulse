using Microsoft.EntityFrameworkCore;
using Pulse.Domain.Entities;

namespace Pulse.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Event> Events { get; }
    DbSet<EventApplication> EventApplications { get; }
    DbSet<ChatMessage> ChatMessages { get; }
    DbSet<Review> Reviews { get; }
    DbSet<Notification> Notifications { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
