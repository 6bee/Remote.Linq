// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server;

using Common.Model;
using Microsoft.EntityFrameworkCore;

public class EFContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlServer(@"Server=.;Database=RemoteQueryableDemoDB_MAY2020;User Id=Demo;Password=demo(!)Password;MultipleActiveResultSets=True;App=Remote.Linq.Demo.EFCore;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
            .HasOne(x => x.ProductCategory)
            .WithMany()
            .HasForeignKey("ProductCategoryId");

        modelBuilder.Entity<Product>()
            .HasMany(x => x.Markets)
            .WithOne();

        modelBuilder.Entity<OrderItem>()
            .HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey("ProductId");

        modelBuilder.Entity<ProductProductGroup>()
            .ToTable("Products_ProductGroups")
            .HasKey("ProductId", "ProductGroupId");

        modelBuilder.Entity<ProductProductGroup>()
            .HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey("ProductId");

        modelBuilder.Entity<ProductProductGroup>()
            .HasOne(x => x.ProductGroup)
            .WithMany()
            .HasForeignKey("ProductGroupId");

        modelBuilder.Entity<ProductGroup>()
            .HasMany(x => x.Products)
            .WithOne();

        modelBuilder.Entity<ProductMarket>()
            .ToTable("Products_Markets")
            .HasKey(x => new { x.ProductId, x.MarketId });

        modelBuilder.Entity<ProductMarket>()
            .HasOne(x => x.Product)
            .WithMany(x => x.Markets)
            .HasForeignKey(x => x.ProductId);

        modelBuilder.Entity<ProductMarket>()
            .HasOne(x => x.Market)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.MarketId);

        modelBuilder.Entity<Market>()
            .HasMany(x => x.Products)
            .WithOne(x => x.Market);
    }

    public DbSet<OrderItem> OrderItems { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<ProductGroup> ProductGroups { get; set; }

    public DbSet<ProductCategory> ProductCategories { get; set; }

    public DbSet<Market> Markets { get; set; }
}