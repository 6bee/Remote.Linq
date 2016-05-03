// Copyright (c) Christof Senn. All rights reserved. 

namespace Server
{
    using Common.Model;
    using System.Data.Entity;

    public partial class EFContext : DbContext
    {
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
    }
}
