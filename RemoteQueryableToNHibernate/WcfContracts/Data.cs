// Copyright (c) Christof Senn. All rights reserved. 

namespace WcfContracts
{
    public class OrderItem
    {
        public virtual int Id { get; set; }
        public virtual int ProductId { get; set; }
        public virtual int Quantity { get; set; }
    }

    public class Product
    {
        public virtual int Id { get; set; }
        public virtual int ProductCategoryId { get; set; }
        public virtual string Name { get; set; }
        public virtual decimal Price { get; set; }
    }

    public class ProductCategory
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
    }
}
