// Copyright (c) Christof Senn. All rights reserved. 

namespace WcfService.Mappings
{
    using FluentNHibernate.Mapping;
    using WcfContracts;

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