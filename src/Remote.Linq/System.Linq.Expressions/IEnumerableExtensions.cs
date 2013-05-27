// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System.Collections.Generic;
using Remote.Linq;

namespace System.Linq.Expressions
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Applies this query instance to an enumerable
        /// </summary>
        /// <param name="queriable"></param>
        /// <returns></returns>
        public static IEnumerable<TEntity> ApplyQuery<TEntity>(this IEnumerable<TEntity> enumerable, Query<TEntity> query)
        {
            return enumerable
                .AsQueryable()
                .ApplyQuery(query)
                .AsEnumerable();
        }
    }
}
