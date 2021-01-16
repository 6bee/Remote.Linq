// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using Common.Model;
    using System;
    using System.Linq;

    public interface IAsyncRemoteRepository : IAsyncDisposable
    {
        IAsyncQueryable<ProductCategory> ProductCategories { get; }

        IAsyncQueryable<Product> Products { get; }

        IAsyncQueryable<OrderItem> OrderItems { get; }

        IAsyncQueryable<ProductGroup> ProductGroups { get; }
    }
}
