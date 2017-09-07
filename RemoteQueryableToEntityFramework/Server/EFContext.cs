// Copyright (c) Christof Senn. All rights reserved. 

namespace Server
{
    using Common.Model;
    using System.Data.Entity;

    public partial class EFContext : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .HasRequired(x => x.ProductCategory)
                .WithMany()
                .Map(m => m.MapKey("ProductCategoryId"));

            modelBuilder.Entity<OrderItem>()
                .HasRequired(x => x.Product)
                .WithMany()
                .Map(m => m.MapKey("ProductId"));

            modelBuilder.Entity<Market>()
                .HasMany(x => x.Products)
                .WithMany(x => x.Markets)
                .Map(x =>
                {
                    x.ToTable("Markets_Products");
                    x.MapLeftKey("MarketId");
                    x.MapRightKey("ProductId");
                });
        }

        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<Market> Markets { get; set; }
    }
}
