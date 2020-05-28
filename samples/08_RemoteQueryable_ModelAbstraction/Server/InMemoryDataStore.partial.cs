// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Common.Model;
    using System.Collections.Generic;
    using System.Linq;

    partial class InMemoryDataStore
    {
        public IEnumerable<IEntity> All
            => Enumerable.Empty<IEntity>()
            .Concat(Products)
            .Concat(ProductGroups)
            .Concat(ProductCategories)
            .Concat(OrderItems);
    }
}
