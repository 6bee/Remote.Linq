// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using System.Linq.Expressions;

    internal sealed class ObjectResultCaster : IQueryResultMapper<object>
    {
        public TResult MapResult<TResult>(object source, Expression expression)
            => (TResult)source;
    }
}
