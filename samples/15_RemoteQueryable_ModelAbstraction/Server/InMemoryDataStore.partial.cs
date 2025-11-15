// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server;

using Common.Model;

partial class InMemoryDataStore
{
    public IQueryable<IEntity> All
        => Enumerable.Empty<IEntity>()
        .AsQueryable()
        .Concat(Products)
        .Concat(ProductGroups)
        .Concat(ProductCategories)
        .Concat(OrderItems);
}