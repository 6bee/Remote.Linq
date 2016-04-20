// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Creates an instance of <see cref="IQueryable{T}" /> that utilizes the data provider specified
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resource"></param>
        /// <param name="dataProvider"></param>
        /// <param name="typeResolver"></param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public static IQueryable<T> AsQueryable<T>(this IEnumerable<T> resource, Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider, ITypeResolver typeResolver = null, IDynamicObjectMapper mapper = null)
        {
            return RemoteQueryable.Create<T>(dataProvider, typeResolver, mapper);
        }

        /// <summary>
        /// Applies this query instance to an enumerable
        /// </summary>
        /// <param name="queriable"></param>
        /// <returns></returns>
        public static IEnumerable<TEntity> ApplyQuery<TEntity>(this IEnumerable<TEntity> enumerable, IQuery<TEntity> query)
        {
            return enumerable
                .AsQueryable()
                .ApplyQuery(query)
                .AsEnumerable();
        }

        /// <summary>
        /// Applies this query instance to an enumerable
        /// </summary>
        /// <param name="queriable"></param>
        /// <returns></returns>
        public static IEnumerable<TEntity> ApplyQuery<TEntity>(this IEnumerable<TEntity> enumerable, IQuery query)
        {
            return enumerable
                .AsQueryable()
                .ApplyQuery(query)
                .AsEnumerable();
        }
    }
}
