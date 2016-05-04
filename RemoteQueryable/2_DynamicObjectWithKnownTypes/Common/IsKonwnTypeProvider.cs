// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common
{
    using Aqua.Dynamic;
    using Model;
    using System;
    using System.Linq;

    public class IsKnownTypeProvider : IIsKnownTypeProvider
    {
        private static readonly Func<Type, bool> _isKnowType = new[] { typeof(OrderItem), typeof(Product), typeof(ProductCategory) }.ToDictionary(x => x).ContainsKey;

        public bool IsKnownType(Type type) => _isKnowType(type);
    }
}
