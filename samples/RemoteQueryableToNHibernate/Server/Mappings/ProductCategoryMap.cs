// Copyright (c) Christof Senn. All rights reserved. 

namespace Server.Mappings
{
    using Common.Model;
    using FluentNHibernate.Mapping;

    public class ProductCategoryMap : ClassMap<ProductCategory>
    {
        public ProductCategoryMap()
        {
            Table("ProductCategories");
            
            Id(x => x.Id);
            
            Map(x => x.Name);
        }
    }
}