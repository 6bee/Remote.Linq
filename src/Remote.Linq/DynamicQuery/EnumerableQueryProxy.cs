// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public sealed class EnumerableQueryProxy<T> : IQueryable<T>
    {
        private readonly IQueryable<T> _queryable;
        
        public EnumerableQueryProxy(IEnumerable<T> enumerable)
        {
            _queryable = enumerable.AsQueryable();
        }

        internal EnumerableQueryProxy(IQueryable<T> queryable)
        {
            _queryable = queryable;
        }

        public Expression Expression => _queryable.Expression;

        public Type ElementType => _queryable.ElementType;

        public IQueryProvider Provider => _queryable.Provider;

        public IEnumerator<T> GetEnumerator() => _queryable.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_queryable).GetEnumerator();
    }
}
