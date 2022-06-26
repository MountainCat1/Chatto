using Microsoft.EntityFrameworkCore;

namespace Chatto.Infrastructure;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TextChannelInvite>()
            .HasOne<User>(b => b.Target)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<TextChannel> TextChannels { get; set; }
    public DbSet<TextChannelInvite> TextChannelInvites { get; set; }
}