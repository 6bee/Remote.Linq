// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Common.Model;
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

            modelBuilder.Entity<Product>()
                .HasRequired(x => x.ProductCategory)
                .WithMany()
                .Map(m => m.MapKey("ProductCategoryId"));

            modelBuilder.Entity<OrderItem>()
                .HasRequired(x => x.Product)
                .WithMany()
                .Map(m => m.MapKey("ProductId"));

            modelBuilder.Entity<ProductGroup>()
                .HasMany(x => x.Products)
                .WithMany()
                .Map(x =>
                {
                    x.ToTable("Products_ProductGroups");
                    x.MapLeftKey("ProductGroupId");
                    x.MapRightKey("ProductId");
                });

            modelBuilder.Entity<Market>()
                .HasMany(x => x.Products)
                .WithMany(x => x.Markets)
                .Map(x =>
                {
                    x.ToTable("Products_Markets");
                    x.MapLeftKey("MarketId");
                    x.MapRightKey("ProductId");
                });
        }

        public virtual DbSet<OrderItem> OrderItems { get; set; }

        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<ProductGroup> ProductGroups { get; set; }

        public virtual DbSet<ProductCategory> ProductCategories { get; set; }

        public virtual DbSet<Market> Markets { get; set; }
    }
}
