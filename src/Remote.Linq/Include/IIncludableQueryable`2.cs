// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Include
{
    using System.Linq;

    public interface IIncludableQueryable<out T, out TProperty> : IQueryable<T>
    {
    }
}