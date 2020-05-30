// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server.Mappings
{
    using Common.Model;
    using FluentNHibernate.Mapping;

    public class OrderItemMap : ClassMap<OrderItem>
    {
        public OrderItemMap()
        {
            Not.LazyLoad();

            Table("OrderItems");

            Id(x => x.Id);

            Map(x => x.ProductId);
            Map(x => x.Quantity);
            Map(x => x.UnitPrice);
        }
    }
}