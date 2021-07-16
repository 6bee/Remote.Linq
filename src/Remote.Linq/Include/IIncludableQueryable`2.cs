// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Include
{
    using System.Linq;

    /// <summary>
    /// Represents the result of an include operation of a remote queryable resource.
    /// </summary>
    /// <typeparam name="T">The type of the data in the data source.</typeparam>
    /// <typeparam name="TProperty">The type of the property to include.</typeparam>
    public interface IIncludableQueryable<out T, out TProperty> : IQueryable<T>
    {
    }
}