// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using System;

    public class QueryableResourceDescriptorProvider : IQueryableResourceDescriptorProvider
    {
        public virtual QueryableResourceDescriptor Get(Type type)
            => new QueryableResourceDescriptor(type);
    }
}
