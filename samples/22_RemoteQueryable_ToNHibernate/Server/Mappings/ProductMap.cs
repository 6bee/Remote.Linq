// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server.Mappings
{
    using Common.Model;
    using FluentNHibernate.Mapping;

    public class ProductMap : ClassMap<Product>
    {
        public ProductMap()
        {
            Not.LazyLoad();

            Table("Products");

            Id(x => x.Id);

            Map(x => x.Name);
            Map(x => x.ProductCategoryId);
            Map(x => x.Price);
        }
    }
}