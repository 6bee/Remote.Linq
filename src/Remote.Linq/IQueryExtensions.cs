// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class IQueryExtensions
    {
        /// <summary>
        /// Creates a generic version of the current query instance.
        /// </summary>
        public static Query<T> ToGenericQuery<T>(this IQuery query, Func<Query<T>, IEnumerable<T>>? dataProvider = null, Func<LambdaExpression, Expressions.LambdaExpression>? expressionTranslator = null, ITypeResolver? typeResolver = null)
        {
            var type = (typeResolver ?? TypeResolver.Instance).ResolveType(query.CheckNotNull(nameof(query)).Type);
            if (typeof(T) != type)
            {
                throw new RemoteLinqException($"Generic type mismatch: {typeof(T)} vs. {query.Type}");
            }

            var instance = new Query<T>(dataProvider, expressionTranslator, query.FilterExpressions, query.SortExpressions, query.SkipValue, query.TakeValue);
            return instance;
        }
    }
}
