// Copyright (c) Christof Senn. All rights reserved. 

using System.Data.Entity;
using WcfContracts;

namespace WcfService
{
    public partial class EFContext : DbContext
    {
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
    }
}
