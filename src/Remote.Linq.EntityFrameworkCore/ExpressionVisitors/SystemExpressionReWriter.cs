// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.ExpressionVisitors
{
    using Microsoft.EntityFrameworkCore;
    using Remote.Linq;
    using Remote.Linq.ExpressionVisitors;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class SystemExpressionReWriter
    {
        /// <summary>
        /// Replaces include query methods by entity framework's include methods.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be examined.</param>
        /// <returns>A new expression tree with the EF's version of the include method if any method calls to include was contained in the original expression,
        /// otherwise the original expression tree is returned.</returns>
        public static Expression ReplaceIncludeQueryMethods(this Expression expression)
            => new QueryMethodMapper().Run(expression);

        private sealed class QueryMethodMapper : ExpressionVisitorBase
        {
            private static class EntityFrameworkMethodInfos
            {
                /// <summary>
                /// Type definition used in generic type filters.
                /// </summary>
                [SuppressMessage("Major Bug", "S3453:Classes should not have only \"private\" constructors", Justification = "For reflection only")]
                private sealed class TEntity
                {
                    private TEntity()
                    {
                    }
                }

                /// <summary>
                /// Type definition used in generic type filters.
                /// </summary>
                [SuppressMessage("Major Bug", "S3453:Classes should not have only \"private\" constructors", Justification = "For reflection only")]
                private sealed class TProperty
                {
                    private TProperty()
                    {
                    }
                }

                /// <summary>
                /// Type definition used in generic type filters.
                /// </summary>
                [SuppressMessage("Major Bug", "S3453:Classes should not have only \"private\" constructors", Justification = "For reflection only")]
                private sealed class TPreviousProperty
                {
                    private TPreviousProperty()
                    {
                    }
                }

                internal static readonly MethodInfo StringIncludeMethodInfo = MethodInfos.GetMethod(
                    typeof(EntityFrameworkQueryableExtensions),
                    nameof(EntityFrameworkQueryableExtensions.Include),
                    new[] { typeof(TEntity) },
                    typeof(IQueryable<TEntity>),
                    typeof(string));

                internal static readonly MethodInfo IncludeMethodInfo = MethodInfos.GetMethod(
                    typeof(EntityFrameworkQueryableExtensions),
                    nameof(EntityFrameworkQueryableExtensions.Include),
                    new[] { typeof(TEntity), typeof(TProperty) },
                    typeof(IQueryable<TEntity>),
                    typeof(Expression<Func<TEntity, TProperty>>));

                internal static readonly MethodInfo ThenIncludeAfterEnumerableMethodInfo = MethodInfos.GetMethod(
                    typeof(EntityFrameworkQueryableExtensions),
                    nameof(EntityFrameworkQueryableExtensions.ThenInclude),
                    new[] { typeof(TEntity), typeof(TPreviousProperty), typeof(TProperty) },
                    typeof(Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<TEntity, IEnumerable<TPreviousProperty>>),
                    typeof(Expression<Func<TPreviousProperty, TProperty>>));

                internal static readonly MethodInfo ThenIncludeAfterReferenceMethodInfo = MethodInfos.GetMethod(
                    typeof(EntityFrameworkQueryableExtensions),
                    nameof(EntityFrameworkQueryableExtensions.ThenInclude),
                    new[] { typeof(TEntity), typeof(TPreviousProperty), typeof(TProperty) },
                    typeof(Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<TEntity, TPreviousProperty>),
                    typeof(Expression<Func<TPreviousProperty, TProperty>>));
            }

            internal Expression Run(Expression expression) => Visit(expression);

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (MapRemoteLinqToEntityFrameworkMethod(node.Method) is MethodInfo mappedMethod)
                {
                    var arguments = node.Arguments.Select(Visit).ToArray();
                    node = Expression.Call(mappedMethod, arguments);
                }

                return base.VisitMethodCall(node);

                static MethodInfo? MapRemoteLinqToEntityFrameworkMethod(MethodInfo? source)
                {
                    if (source is null)
                    {
                        return null;
                    }

                    if (source.DeclaringType != typeof(IncludeQueryableExtensions))
                    {
                        return null;
                    }

                    var target = MapMethod(source);
                    if (target is null)
                    {
                        return null;
                    }

                    var genericArguments = source.GetGenericArguments();
                    return target.MakeGenericMethod(genericArguments);

                    static MethodInfo? MapMethod(MethodInfo source)
                    {
                        var method = source.GetGenericMethodDefinition();

                        if (method == IncludeQueryableExtensions.IncludeMethodInfo)
                        {
                            return EntityFrameworkMethodInfos.IncludeMethodInfo;
                        }

                        if (method == IncludeQueryableExtensions.StringIncludeMethodInfo)
                        {
                            return EntityFrameworkMethodInfos.StringIncludeMethodInfo;
                        }

                        if (method == IncludeQueryableExtensions.ThenIncludeAfterEnumerableMethodInfo)
                        {
                            return EntityFrameworkMethodInfos.ThenIncludeAfterEnumerableMethodInfo;
                        }

                        if (method == IncludeQueryableExtensions.ThenIncludeAfterReferenceMethodInfo)
                        {
                            return EntityFrameworkMethodInfos.ThenIncludeAfterReferenceMethodInfo;
                        }

                        return null;
                    }
                }
            }
        }
    }
}