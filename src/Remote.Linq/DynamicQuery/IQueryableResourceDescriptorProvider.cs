// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using System;

    public interface IQueryableResourceDescriptorProvider
    {
        QueryableResourceDescriptor Get(Type type);
    }
}
