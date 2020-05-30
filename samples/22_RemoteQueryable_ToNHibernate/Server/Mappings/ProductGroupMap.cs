// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server.Mappings
{
    using Common.Model;
    using FluentNHibernate.Mapping;

    public class ProductGroupMap : ClassMap<ProductGroup>
    {
        public ProductGroupMap()
        {
            Not.LazyLoad();

            Table("ProductGroups");

            Id(x => x.Id);

            Map(x => x.GroupName);
        }
    }
}