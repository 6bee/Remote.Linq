// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server.DbModel;

public class ProductEntity
{
    public int Id { get; set; }

    public ProductCategoryEntity ProductCategory { get; set; }

    public string Name { get; set; }

    public decimal Price { get; set; }

    public IList<MarketEntity> Markets { get; set; }
}