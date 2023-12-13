// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Server.DbModel;
    using System.Data.Entity;

    public partial class EFContext : DbContext
    {
        public EFContext()
            : base("data source=.;initial catalog=RemoteQueryableDemoDB_MAY2020;User Id=Demo;Password=demo(!)Password;MultipleActiveResultSets=True;App=Remote.Linq.Demo.EF6;")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductEntity>()
                .ToTable("Products")
                .HasRequired(x => x.ProductCategory)
                .WithMany()
                .Map(m => m.MapKey("ProductCategoryId"));

            modelBuilder.Entity<OrderItemEntity>()
                .ToTable("OrderItems")
                .HasRequired(x => x.Product)
                .WithMany()
                .Map(m => m.MapKey("ProductId"));

            modelBuilder.Entity<ProductGroupEntity>()
                .ToTable("ProductGroups")
                .HasMany(x => x.Products)
                .WithMany()
                .Map(x =>
                {
                    x.ToTable("Products_ProductGroups");
                    x.MapLeftKey("ProductGroupId");
                    x.MapRightKey("ProductId");
                });

            modelBuilder.Entity<ProductCategoryEntity>()
                .ToTable("ProductCategories");

            modelBuilder.Entity<MarketEntity>()
                .ToTable("Markets")
                .HasMany(x => x.Products)
                .WithMany(x => x.Markets)
                .Map(x =>
                {
                    x.ToTable("Products_Markets");
                    x.MapLeftKey("MarketId");
                    x.MapRightKey("ProductId");
                });
        }

        public virtual DbSet<OrderItemEntity> OrderItems { get; set; }

        public virtual DbSet<ProductEntity> Products { get; set; }

        public virtual DbSet<ProductGroupEntity> ProductGroups { get; set; }

        public virtual DbSet<ProductCategoryEntity> ProductCategories { get; set; }

        public virtual DbSet<MarketEntity> Markets { get; set; }
    }
}