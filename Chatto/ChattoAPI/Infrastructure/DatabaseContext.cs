﻿using Microsoft.EntityFrameworkCore;

namespace Chatto.Infrastructure;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<TextChannel> TextChannels { get; set; }
    
    
    public DbSet<TextChannelInvite> TextChannelInvites { get; set; }
}