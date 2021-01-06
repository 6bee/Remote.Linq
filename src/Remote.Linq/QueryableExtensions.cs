// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.ExpressionVisitors;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using MethodInfo = System.Reflection.MethodInfo;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class QueryableExtensions
    {
        private static readonly Func<Expressions.LambdaExpression, Expressions.LambdaExpression> _defaultExpressionVisitor = RemoteExpressionReWriter.ReplaceNonGenericQueryArgumentsByGenericArguments;

        /// <summary>
        /// Creates an instance of <see cref="IQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        public static IQueryable<T> AsQueryable<T>(this IQueryable<T> resource, Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider, ITypeInfoProvider typeInfoProvider = null, IDynamicObjectMapper mapper = null)
            => RemoteQueryable.Factory.CreateQueryable<T>(dataProvider, typeInfoProvider, mapper);

        /// <summary>
        /// Creates an instance of <see cref="IQueryable" /> that utilizes the data provider specified.
        /// </summary>
        [Obsolete("This method will be removed in future versions. Use AsQueryable() without generic argument instead.", false)]
        public static IQueryable AsQueryable<T>(this IQueryable resource, Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider, ITypeInfoProvider typeInfoProvider = null, IDynamicObjectMapper mapper = null)
            => RemoteQueryable.Factory.CreateQueryable(resource.ElementType, dataProvider, typeInfoProvider, mapper);

        /// <summary>
        /// Creates an instance of <see cref="IQueryable" /> that utilizes the data provider specified.
        /// </summary>
        public static IQueryable AsQueryable(this IQueryable resource, Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider, ITypeInfoProvider typeInfoProvider = null, IDynamicObjectMapper mapper = null)
            => RemoteQueryable.Factory.CreateQueryable(resource.ElementType, dataProvider, typeInfoProvider, mapper);

        private static IOrderedQueryable<T> Sort<T>(this IQueryable<T> queryable, LambdaExpression lambdaExpression, MethodInfo methodInfo)
        {
            var exp = lambdaExpression.Body;
            var resultType = exp.Type;
            var funcType = typeof(Func<,>).MakeGenericType(typeof(T), resultType);
            var lambdaExpressionMethodInfo = MethodInfos.Expression.Lambda.MakeGenericMethod(funcType);

            var funcExpression = lambdaExpressionMethodInfo.Invoke(null, new object[] { exp, lambdaExpression.Parameters.ToArray() });

            var method = methodInfo.MakeGenericMethod(typeof(T), resultType);
            var result = method.Invoke(null, new object[] { queryable, funcExpression });

            return (IOrderedQueryable<T>)result;
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> queryable, LambdaExpression lambdaExpression)
        {
            return queryable.Sort<T>(lambdaExpression, MethodInfos.Queryable.OrderBy);
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> queryable, LambdaExpression lambdaExpression)
        {
            return queryable.Sort<T>(lambdaExpression, MethodInfos.Queryable.OrderByDescending);
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> queryable, LambdaExpression lambdaExpression)
        {
            return queryable.Sort<T>(lambdaExpression, MethodInfos.Queryable.ThenBy);
        }

        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> queryable, LambdaExpression lambdaExpression)
        {
            return queryable.Sort<T>(lambdaExpression, MethodInfos.Queryable.ThenByDescending);
        }

        /// <summary>
        /// Applies this query instance to a queryable.
        /// </summary>
        public static IQueryable<TEntity> ApplyQuery<TEntity>(this IQueryable<TEntity> queryable, IQuery<TEntity> query, Func<Expressions.LambdaExpression, Expressions.LambdaExpression> expressionVisitor)
        {
            var visitor = expressionVisitor ?? _defaultExpressionVisitor;
            return queryable
                .ApplyFilters(query, visitor)
                .ApplySorting(query, visitor)
                .ApplyPaging(query);
        }

        /// <summary>
        /// Applies this query instance to a queryable.
        /// </summary>
        public static IQueryable<TEntity> ApplyQuery<TEntity>(this IQueryable<TEntity> queryable, IQuery query, Func<Expressions.LambdaExpression, Expressions.LambdaExpression> expressionVisitor)
        {
            var q = Query<TEntity>.CreateFromNonGeneric(query);
            return queryable.ApplyQuery(q, expressionVisitor ?? _defaultExpressionVisitor);
        }

        private static IQueryable<T> ApplyFilters<T>(this IQueryable<T> queriable, IQuery<T> query, Func<Expressions.LambdaExpression, Expressions.LambdaExpression> expressionVisitor)
        {
            foreach (var filter in query.FilterExpressions)
            {
                var predicate = expressionVisitor(filter).ToLinqExpression<T, bool>();
                queriable = queriable.Where(predicate);
            }

            return queriable;
        }

        private static IQueryable<T> ApplySorting<T>(this IQueryable<T> queriable, IQuery<T> query, Func<Expressions.LambdaExpression, Expressions.LambdaExpression> expressionVisitor)
        {
            IOrderedQueryable<T> orderedQueriable = null;
            foreach (var sort in query.SortExpressions)
            {
                var exp = expressionVisitor(sort.Operand).ToLinqExpression();
                if (orderedQueriable is null)
                {
                    switch (sort.SortDirection)
                    {
                        case Expressions.SortDirection.Ascending:
                            orderedQueriable = queriable.OrderBy(exp);
                            break;
                        case Expressions.SortDirection.Descending:
                            orderedQueriable = queriable.OrderByDescending(exp);
                            break;
                    }
                }
                else
                {
                    switch (sort.SortDirection)
                    {
                        case Expressions.SortDirection.Ascending:
                            orderedQueriable = orderedQueriable.ThenBy(exp);
                            break;
                        case Expressions.SortDirection.Descending:
                            orderedQueriable = orderedQueriable.ThenByDescending(exp);
                            break;
                    }
                }
            }

            return orderedQueriable ?? queriable;
        }

        private static IQueryable<T> ApplyPaging<T>(this IQueryable<T> queriable, IQuery<T> query)
        {
            if (query.SkipValue.HasValue)
            {
                queriable = queriable.Skip(query.SkipValue.Value);
            }

            if (query.TakeValue.HasValue)
            {
                queriable = queriable.Take(query.TakeValue.Value);
            }

            return queriable;
        }

        public static IQueryable<T> Include<T>(this IQueryable<T> queryable, string navigationPropertyPath)
        {
            if (queryable is null)
            {
                throw new ArgumentNullException(nameof(queryable));
            }

            if (navigationPropertyPath is null)
            {
                throw new ArgumentNullException(nameof(navigationPropertyPath));
            }

            if (string.IsNullOrEmpty(navigationPropertyPath))
            {
                throw new ArgumentException("path must not be empty", nameof(navigationPropertyPath));
            }

            if (queryable is IRemoteQueryable<T>)
            {
                var methodInfo = MethodInfos.QueryFuntion.Include.MakeGenericMethod(typeof(T));
                var parameter1 = queryable.Expression;
                var parameter2 = Expression.Constant(navigationPropertyPath);
                var methodCallExpression = Expression.Call(methodInfo, parameter1, parameter2);
                var newQueryable = queryable.Provider.CreateQuery<T>(methodCallExpression);
                return newQueryable;
            }

            return queryable;
        }

        public static IIncludableRemoteQueryable<T, TProperty> Include<T, TProperty>(this IQueryable<T> queryable, Expression<Func<T, TProperty>> navigationPropertyPath)
        {
            if (queryable is null)
            {
                throw new ArgumentNullException(nameof(queryable));
            }

            if (queryable is IRemoteQueryable<T> remoteQueryable)
            {
                return IncludeCore(remoteQueryable, navigationPropertyPath, false);
            }

            throw new ArgumentException($"{nameof(Include)} is supported for IRemoteQueryable<> only.", nameof(queryable));
        }

        public static IIncludableRemoteQueryable<T, TProperty> ThenInclude<T, TPreviousProperty, TProperty>(this IIncludableRemoteQueryable<T, TPreviousProperty> queryable, Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
            => IncludeCore(queryable, navigationPropertyPath, true);

        public static IIncludableRemoteQueryable<T, TProperty> ThenInclude<T, TPreviousProperty, TProperty>(this IIncludableRemoteQueryable<T, IEnumerable<TPreviousProperty>> queryable, Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
            => IncludeCore(queryable, navigationPropertyPath, true);

        private static IIncludableRemoteQueryable<T, TNavigationProperty> IncludeCore<T, TNavigationSource, TNavigationProperty>(IRemoteQueryable<T> queryable, Expression<Func<TNavigationSource, TNavigationProperty>> navigationPropertyPath, bool isNestedStatement)
        {
            if (queryable is null)
            {
                throw new ArgumentNullException(nameof(queryable));
            }

            if (navigationPropertyPath is null)
            {
                throw new ArgumentNullException(nameof(navigationPropertyPath));
            }

            if (!TryParsePath(navigationPropertyPath.Body, out string path) || path is null)
            {
                throw new ArgumentException("Invalid include path expression", nameof(navigationPropertyPath));
            }

            var queryableBase = queryable;
            if (isNestedStatement && queryable is IStackedIncludableQueryable<T> preceding)
            {
                queryableBase = preceding.Parent;
                path = $"{preceding.IncludePath}.{path}";
            }

            var includeExpresson = queryableBase.Include(path);
            return new IncludableRemoteQueryable<T, TNavigationProperty>(queryable.Provider, includeExpresson.Expression, queryableBase, path);
        }

        private static bool TryParsePath(Expression expression, out string path)
        {
            path = null;
            var expression1 = RemoveConvert(expression);

            if (expression1 is MemberExpression memberExpression)
            {
                var name = memberExpression.Member.Name;
                if (!TryParsePath(memberExpression.Expression, out var path1))
                {
                    return false;
                }

                path = path1 is null ? name : $"{path1}.{name}";
            }
            else if (expression1 is MethodCallExpression methodCallExpression)
            {
                if (methodCallExpression.Method.Name == "Select" && methodCallExpression.Arguments.Count == 2 && (TryParsePath(methodCallExpression.Arguments[0], out var path1) && path1 != null))
                {
                    var lambdaExpression = methodCallExpression.Arguments[1] as LambdaExpression;
                    if (lambdaExpression != null && TryParsePath(lambdaExpression.Body, out var path2) && path2 != null)
                    {
                        path = $"{path1}.{path2}";
                        return true;
                    }
                }

                return false;
            }

            return true;
        }

        private static Expression RemoveConvert(Expression expression)
        {
            while (expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked)
            {
                expression = ((UnaryExpression)expression).Operand;
            }

            return expression;
        }
    }
}
