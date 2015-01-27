// Copyright (c) Christof Senn. All rights reserved. 

namespace WcfService.Mappings
{
    using FluentNHibernate.Mapping;
    using WcfContracts;

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