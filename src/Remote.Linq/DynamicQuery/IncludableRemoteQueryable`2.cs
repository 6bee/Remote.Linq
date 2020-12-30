// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using System.Linq.Expressions;

    internal class IncludableRemoteQueryable<T, TProperty> : RemoteQueryable<T>, IIncludableRemoteQueryable<T, TProperty>, IStackedIncludableQueryable<T>
    {
        internal IncludableRemoteQueryable(IRemoteQueryProvider provider, Expression expression, IRemoteQueryable<T> parent, string includePath)
            : base(provider, expression)
        {
            Parent = parent;
            IncludePath = includePath;
        }

        public IRemoteQueryable<T> Parent { get; }

        public string IncludePath { get; }
    }
}
