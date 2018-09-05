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
        /// Creates an instance of <see cref="IQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        public static IQueryable<T> AsQueryable<T>(this IEnumerable<T> resource, Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider, ITypeInfoProvider typeInfoProvider = null, IDynamicObjectMapper mapper = null)
            => RemoteQueryable.Factory.CreateQueryable<T>(dataProvider, typeInfoProvider, mapper);

        /// <summary>
        /// Applies this query instance to an enumerable.
        /// </summary>
        public static IEnumerable<TEntity> ApplyQuery<TEntity>(this IEnumerable<TEntity> enumerable, IQuery<TEntity> query, Func<Expressions.LambdaExpression, Expressions.LambdaExpression> expressionVisitor = null)
            => enumerable
                .AsQueryable()
                .ApplyQuery(query, expressionVisitor)
                .AsEnumerable();

        /// <summary>
        /// Applies this query instance to an enumerable.
        /// </summary>
        public static IEnumerable<TEntity> ApplyQuery<TEntity>(this IEnumerable<TEntity> enumerable, IQuery query, Func<Expressions.LambdaExpression, Expressions.LambdaExpression> expressionVisitor = null)
            => enumerable
                .AsQueryable()
                .ApplyQuery(query, expressionVisitor)
                .AsEnumerable();
    }
}
