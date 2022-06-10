﻿using Microsoft.EntityFrameworkCore;

namespace ChattoAuth.Entities;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }
    public DbSet<Account> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Account>()
            .HasDiscriminator<string>("AccountType")
            .HasValue<GoogleAccount>("GoogleId");
    }
}