// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework.Tests.TestModel;

using System.Data.Entity;

public class TestContext : DbContext
{
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LookupItem>().HasKey(x => x.Key);
        }

        public virtual DbSet<LookupItem> Items { get; set; }
    }