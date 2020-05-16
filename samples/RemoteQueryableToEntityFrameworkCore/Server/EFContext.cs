// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Common.Model;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;

    public class EFContext : DbContext
    {
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductCategory> ProductCategories { get; set; }

        internal IQueryable Set(Type type)
        {
            System.Reflection.MethodInfo genericSetMethod = GetType().GetMethods().Single(x => x.Name == "Set" && x.IsGenericMethod).MakeGenericMethod(type);

            return (IQueryable)genericSetMethod.Invoke(this, null);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer(@"Server=.;Database=RemoteQueryableDemoDB_AUG2017;User Id=Demo;Password=demo(!)Password;");
        }

        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //    modelBuilder.Entity<OrderItem>();
        //    modelBuilder.Entity<Product>();
        //    modelBuilder.Entity<ProductCategory>();
        // }
    }
}
