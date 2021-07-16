// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using System;
    using System.Linq.Expressions;

    public class AsyncRemoteQueryable : RemoteQueryable, IOrderedAsyncRemoteQueryable
    {
        public AsyncRemoteQueryable(Type elemntType, IAsyncRemoteQueryProvider provider, Expression? expression = null)
            : base(elemntType, provider, expression)
        {
        }

        /// <inheritdoc/>
        public new IAsyncRemoteQueryProvider Provider => (IAsyncRemoteQueryProvider)base.Provider;
    }
}
