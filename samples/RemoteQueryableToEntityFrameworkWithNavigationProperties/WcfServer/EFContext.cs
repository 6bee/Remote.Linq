// Copyright (c) Christof Senn. All rights reserved. 

namespace WcfService
{
    using System.Data.Entity;
    using WcfContracts;

    public partial class EFContext : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
