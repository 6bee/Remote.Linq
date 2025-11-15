// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common;

using Aqua.Dynamic;
using Common.Model;

public class IsKnownTypeProvider : IIsKnownTypeProvider
{
    private static readonly Func<Type, bool> _isKnowType = new HashSet<Type>(new[]
        {
            typeof(OrderItem),
            typeof(Product),
            typeof(ProductCategory),
            typeof(ProductGroup),
        })
        .Contains;

    public bool IsKnownType(Type type) => _isKnowType(type);
}