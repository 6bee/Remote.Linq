// Copyright (c) Christof Senn. All rights reserved. 

namespace Server.Mappings
{
    using Common.Model;
    using FluentNHibernate.Mapping;

    public class OrderItemMap : ClassMap<OrderItem>
    {
        public OrderItemMap()
        {
            Table("OrderItems");
            
            Id(x => x.Id);
            
            Map(x => x.ProductId);
            Map(x => x.Quantity);
        }
    }
}