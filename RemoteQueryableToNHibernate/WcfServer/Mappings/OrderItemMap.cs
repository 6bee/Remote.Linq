// Copyright (c) Christof Senn. All rights reserved. 

namespace WcfService.Mappings
{
    using FluentNHibernate.Mapping;
    using WcfContracts;

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