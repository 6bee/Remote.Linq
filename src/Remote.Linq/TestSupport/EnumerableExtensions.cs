// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.TestSupport
{
    using Remote.Linq.DynamicQuery;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class EnumerableExtensions
    {
        /// <summary>
        /// !!! For unit testing only !!! <br />
        /// Creates an <see cref="IAsyncRemoteQueryable{T}"/> type wrapping test data.
        /// </summary>
        [return: NotNullIfNotNull("source")]
        public static IAsyncRemoteQueryable<T>? AsRemoteQueryable<T>(this IEnumerable<T>? source)
            => source is null
                ? null
                : source is IAsyncRemoteQueryable<T> remoteQueryable
                    ? remoteQueryable
                    : new AsyncRemoteQueryable<T>(new TaskAsyncQueryProvider(), source.AsQueryable().Expression);
    }
}
