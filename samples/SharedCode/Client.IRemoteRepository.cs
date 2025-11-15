// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client;

using Common.Model;

public interface IRemoteRepository : IDisposable
{
    IQueryable<ProductCategory> ProductCategories { get; }

    IQueryable<Product> Products { get; }

    IQueryable<OrderItem> OrderItems { get; }

    IQueryable<ProductGroup> ProductGroups { get; }
}