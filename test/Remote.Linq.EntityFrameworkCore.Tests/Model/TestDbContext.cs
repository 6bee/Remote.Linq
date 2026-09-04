// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.Tests.Model;

using Microsoft.EntityFrameworkCore;
using System;
using System.Security;

[SecuritySafeCritical]
public class TestDbContext : DbContext
{
    public TestDbContext()
        : this(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options)
    {
    }

    public TestDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LookupItem>().HasKey(x => x.Key);
    }

    public DbSet<LookupItem> Lookup { get; set; }
}
