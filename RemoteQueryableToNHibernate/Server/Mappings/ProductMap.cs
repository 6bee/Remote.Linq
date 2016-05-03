// Copyright (c) Christof Senn. All rights reserved. 

namespace Server.Mappings
{
    using Common.Model;
    using FluentNHibernate.Mapping;

    public class ProductMap : ClassMap<Product>
    {
        public ProductMap()
        {
            Table("Products");
            
            Id(x => x.Id);
            
            Map(x => x.Name);
            Map(x => x.ProductCategoryId);
            Map(x => x.Price);
        }
    }
}