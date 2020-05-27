// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

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