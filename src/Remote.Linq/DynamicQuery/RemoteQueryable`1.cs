// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// Provides functionality to compose queries for remote execution.
    /// </summary>
    public class RemoteQueryable<T> : RemoteQueryable, IOrderedRemoteQueryable<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteQueryable{T}"/> class.
        /// </summary>
        public RemoteQueryable(IRemoteQueryProvider provider, Expression? expression = null)
            : base(typeof(T), provider, expression)
        {
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
            => Execute().GetEnumerator();

        /// <inheritdoc/>
        public IEnumerable<T> Execute()
            => Provider.Execute<IEnumerable<T>>(Expression);
    }
}