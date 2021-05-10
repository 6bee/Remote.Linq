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
        private const string EnumerableOfDynamicObjectBasedMethodObsolete = "This method can no longer be used and will be removed in a future version. Make sure to create data provider returning DynamicObject rather than IEnumerable<DynamicObject>.";

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(EnumerableOfDynamicObjectBasedMethodObsolete, true)]
        public static IQueryable<T> AsQueryable<T>(this IEnumerable<T> resource, Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null)
            => throw new NotSupportedException(EnumerableOfDynamicObjectBasedMethodObsolete);

        /// <summary>
        /// Creates an instance of <see cref="IQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This method will be removed in a future version. Use one of the factory methods based on RemoteQueryable.", false)]
        public static IQueryable<T> AsQueryable<T>(this IEnumerable<T> resource, Func<Expressions.Expression, DynamicObject> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null)
            => RemoteQueryable.Factory.CreateQueryable<T>(dataProvider, typeInfoProvider, mapper);

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