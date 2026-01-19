// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.ExpressionVisitors;

using Aqua.TypeExtensions;
using Microsoft.EntityFrameworkCore;
using Remote.Linq.ExpressionVisitors;
using Remote.Linq.Include;
using System.ComponentModel;
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

    /// <summary>
    /// Wrap all queryable contained in <see cref="ConstantExpression"/> in closure as required by EF Core to successfully execute subqueries.
    /// </summary>
    /// <param name="expression">The <see cref="Expression"/> to be processed.</param>
    /// <returns>A new expression tree with queryables wrapped in closures if any queryable was contained in the original expression,
    /// otherwise the original expression tree is returned.</returns>
    public static Expression WrapQueryableInClosure(this Expression expression)
        => new QueryableVisitor().Run(expression);

    private sealed class QueryMethodMapper : SystemExpressionVisitorBase
    {
        private static class EntityFrameworkMethodInfos
        {
            /// <summary>
            /// Type definition used in generic type filters.
            /// </summary>
            private sealed class TEntity;

            /// <summary>
            /// Type definition used in generic type filters.
            /// </summary>
            private sealed class TProperty;

            /// <summary>
            /// Type definition used in generic type filters.
            /// </summary>
            private sealed class TPreviousProperty;

            internal static readonly MethodInfo StringIncludeMethodInfo = typeof(EntityFrameworkQueryableExtensions).GetMethodEx(
                nameof(EntityFrameworkQueryableExtensions.Include),
                [typeof(TEntity)],
                typeof(IQueryable<TEntity>),
                typeof(string));

            internal static readonly MethodInfo IncludeMethodInfo = typeof(EntityFrameworkQueryableExtensions).GetMethodEx(
                nameof(EntityFrameworkQueryableExtensions.Include),
                [typeof(TEntity), typeof(TProperty)],
                typeof(IQueryable<TEntity>),
                typeof(Expression<Func<TEntity, TProperty>>));

            internal static readonly MethodInfo ThenIncludeAfterEnumerableMethodInfo = typeof(EntityFrameworkQueryableExtensions).GetMethodEx(
                nameof(EntityFrameworkQueryableExtensions.ThenInclude),
                [typeof(TEntity), typeof(TPreviousProperty), typeof(TProperty)],
                typeof(Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<TEntity, IEnumerable<TPreviousProperty>>),
                typeof(Expression<Func<TPreviousProperty, TProperty>>));

            internal static readonly MethodInfo ThenIncludeAfterReferenceMethodInfo = typeof(EntityFrameworkQueryableExtensions).GetMethodEx(
                nameof(EntityFrameworkQueryableExtensions.ThenInclude),
                [typeof(TEntity), typeof(TPreviousProperty), typeof(TProperty)],
                typeof(Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<TEntity, TPreviousProperty>),
                typeof(Expression<Func<TPreviousProperty, TProperty>>));
        }

        internal Expression Run(Expression expression) => Visit(expression);

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (MapRemoteLinqToEntityFrameworkMethod(node.Method) is MethodInfo mappedMethod)
            {
                var arguments = node.Arguments.Select(Visit).ToArray();
                node = Expression.Call(mappedMethod, arguments!);
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

    private sealed class QueryableVisitor : SystemExpressionVisitorBase
    {
        internal Expression Run(Expression expression) => Visit(expression);

        protected override Expression VisitConstant(ConstantExpression node)
        {
            // to support EF Core subqueries, queryables must no be returned as ConstantExpression but wrapped as closure.
            if (node.Type.Implements(typeof(IQueryable<>), out var genericargs))
            {
                var queryableType = typeof(IQueryable<>).MakeGenericType(genericargs);
                var closure = Activator.CreateInstance(typeof(Closure<>).MakeGenericType(queryableType), node.Value)
                    ?? throw new RemoteLinqException($"Failed to create closure for type {queryableType}.");
                var valueProperty = closure.GetType().GetProperty(nameof(Closure<>.Value))
                    ?? throw new RemoteLinqException("Failed to get 'Closure.Value' property info.");
                return Expression.MakeMemberAccess(Expression.Constant(closure), valueProperty);
            }

            return base.VisitConstant(node);
        }
    }
}