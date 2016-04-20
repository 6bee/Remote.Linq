// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    internal sealed class ObjectResultCaster : IQueryResultMapper<object>
    {
        public TResult MapResult<TResult>(object source)
        {
            return (TResult)source;
        }
    }
}
