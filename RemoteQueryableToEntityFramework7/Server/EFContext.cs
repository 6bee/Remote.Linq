// Copyright (c) Christof Senn. All rights reserved. 

namespace Server
{
    using Common.Model;
    using Microsoft.Data.Entity;
    using System;
    using System.Linq;

    public class EFContext : DbContext
    {
        public DbSet<OrderItem> OrderItems { get; set; }
        
        public DbSet<Product> Products { get; set; }
        
        public DbSet<ProductCategory> ProductCategories { get; set; }

        internal IQueryable Set(Type type)
        {
            var genericSetMethod = GetType().GetMethods().Single(x => x.Name == "Set" && x.IsGenericMethod).MakeGenericMethod(type);

            return (IQueryable)genericSetMethod.Invoke(this, null); 
        }

        protected override void OnConfiguring(EntityOptionsBuilder optionsBuilder)
        {
            // Visual Studio 2015 | Use the LocalDb 12 instance created by Visual Studio
            // optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=RemoteQueryableDemoDB_AUG2014;Trusted_Connection=True;");

            // Visual Studio 2013 | Use the LocalDb 11 instance created by Visual Studio
            //optionsBuilder.UseSqlServer(@"Server=(localdb)\v11.0;Database=RemoteQueryableDemoDB_AUG2014;Trusted_Connection=True;");

            // Visual Studio 2012 | Use the SQL Express instance created by Visual Studio
            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=RemoteQueryableDemoDB_AUG2014;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Make Blog.Url required
            modelBuilder.Entity<OrderItem>()
                .Table("OrderItems");

            modelBuilder.Entity<Product>()
                .Table("Products");

            modelBuilder.Entity<ProductCategory>()
                .Table("ProductCategories");
        }
    }
}
