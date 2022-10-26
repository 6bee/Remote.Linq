// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.SimpleQuery
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class SimpleQueryEnumerableExtensions
    {
        /// <summary>
        /// Applies this query instance to an enumerable.
        /// </summary>
        public static IEnumerable<TEntity> ApplyQuery<TEntity>(
            this IEnumerable<TEntity> enumerable,
            IQuery<TEntity> query,
            Func<Expressions.LambdaExpression, Expressions.LambdaExpression>? expressionVisitor = null)
            => enumerable
            .AsQueryable()
            .ApplyQuery(query, expressionVisitor);

        /// <summary>
        /// Applies this query instance to an enumerable.
        /// </summary>
        public static IEnumerable<TEntity> ApplyQuery<TEntity>(
            this IEnumerable<TEntity> enumerable,
            IQuery query,
            Func<Expressions.LambdaExpression, Expressions.LambdaExpression>? expressionVisitor = null)
            => enumerable
            .AsQueryable()
            .ApplyQuery(query, expressionVisitor);
    }
}