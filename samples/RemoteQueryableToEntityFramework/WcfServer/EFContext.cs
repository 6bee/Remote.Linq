// Copyright (c) Christof Senn. All rights reserved. 

namespace WcfService
{
    using System.Data.Entity;
    using WcfContracts;

    public partial class EFContext : DbContext
    {
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
    }
}
