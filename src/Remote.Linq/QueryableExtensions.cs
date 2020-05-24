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

        [Obsolete("Use Remote.Linq.RemoteQueryable.Factory.CreateQueryable() to create remote queryable.", true)]
        public static IQueryable<T> AsQueryable<T>(this IQueryable<T> resource, Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null)
            => throw new NotImplementedException();

        [Obsolete("Use Remote.Linq.RemoteQueryable.Factory.CreateQueryable() to create remote queryable.", true)]
        public static IQueryable AsQueryable(this IQueryable resource, Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null)
            => throw new NotImplementedException();

        /// <summary>
        /// Execute the <see cref="IQueryable"/> and return the result without any extra tranformation.
        /// </summary>
        public static TResult Execute<TResult>(this IQueryable source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source.Provider.Execute<TResult>(source.Expression);
        }

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
            => queryable.Sort<T>(lambdaExpression, MethodInfos.Queryable.OrderBy);

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> queryable, LambdaExpression lambdaExpression)
            => queryable.Sort<T>(lambdaExpression, MethodInfos.Queryable.OrderByDescending);

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> queryable, LambdaExpression lambdaExpression)
            => queryable.Sort<T>(lambdaExpression, MethodInfos.Queryable.ThenBy);

        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> queryable, LambdaExpression lambdaExpression)
            => queryable.Sort<T>(lambdaExpression, MethodInfos.Queryable.ThenByDescending);

        /// <summary>
        /// Applies this query instance to a queryable.
        /// </summary>
        public static IQueryable<T> ApplyQuery<T>(this IQueryable<T> queryable, IQuery<T> query, Func<Expressions.LambdaExpression, Expressions.LambdaExpression>? expressionVisitor)
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
        public static IQueryable<T> ApplyQuery<T>(this IQueryable<T> queryable, IQuery query, Func<Expressions.LambdaExpression, Expressions.LambdaExpression>? expressionVisitor)
        {
            var q = Query<T>.CreateFromNonGeneric(query);
            return queryable.ApplyQuery(q, expressionVisitor ?? _defaultExpressionVisitor);
        }

        private static IQueryable<T> ApplyFilters<T>(this IQueryable<T> queriable, IQuery<T> query, Func<Expressions.LambdaExpression, Expressions.LambdaExpression> expressionVisitor)
        {
            foreach (var filter in query.FilterExpressions ?? Enumerable.Empty<Expressions.LambdaExpression>())
            {
                var predicate = expressionVisitor(filter).ToLinqExpression<T, bool>();
                queriable = queriable.Where(predicate);
            }

            return queriable;
        }

        private static IQueryable<T> ApplySorting<T>(this IQueryable<T> queriable, IQuery<T> query, Func<Expressions.LambdaExpression, Expressions.LambdaExpression> expressionVisitor)
        {
            IOrderedQueryable<T>? orderedQueriable = null;
            foreach (var sort in query.SortExpressions ?? Enumerable.Empty<Expressions.SortExpression>())
            {
                var exp = expressionVisitor(sort.Operand).ToLinqExpression();
                if (orderedQueriable is null)
                {
                    orderedQueriable = sort.SortDirection switch
                    {
                        Expressions.SortDirection.Ascending => queriable.OrderBy(exp),
                        Expressions.SortDirection.Descending => queriable.OrderByDescending(exp),
                        _ => throw new RemoteLinqException("not reachable code"),
                    };
                }
                else
                {
                    orderedQueriable = sort.SortDirection switch
                    {
                        Expressions.SortDirection.Ascending => orderedQueriable.ThenBy(exp),
                        Expressions.SortDirection.Descending => orderedQueriable.ThenByDescending(exp),
                        _ => throw new RemoteLinqException("not reachable code"),
                    };
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

        public static IQueryable<T> Include<T>(this IQueryable<T> queryable, string path)
        {
            if (queryable is null)
            {
                throw new ArgumentNullException(nameof(queryable));
            }

            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("path must not be empty", nameof(path));
            }

            if (queryable is IRemoteQueryable<T>)
            {
                var methodInfo = MethodInfos.QueryFuntion.Include.MakeGenericMethod(typeof(T));
                var parameter1 = queryable.Expression;
                var parameter2 = Expression.Constant(path);
                var methodCallExpression = Expression.Call(methodInfo, parameter1, parameter2);
                var newQueryable = queryable.Provider.CreateQuery<T>(methodCallExpression);
                queryable = newQueryable;
            }

            return queryable;
        }

        public static IQueryable<T> Include<T, TProperty>(this IQueryable<T> queryable, Expression<Func<T, TProperty>> path)
        {
            if (queryable is null)
            {
                throw new ArgumentNullException(nameof(queryable));
            }

            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (TryParsePath(path.Body, out var path1) && path1 != null)
            {
                return queryable.Include(path1);
            }

            throw new ArgumentException("Invalid include path expression", nameof(path));
        }

        private static bool TryParsePath(Expression expression, out string? path)
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
                if (string.Equals(methodCallExpression.Method.Name, "Select", StringComparison.Ordinal) &&
                    methodCallExpression.Arguments.Count == 2 &&
                    TryParsePath(methodCallExpression.Arguments[0], out var path1) &&
                    path1 != null &&
                    methodCallExpression.Arguments[1] is LambdaExpression lambdaExpression &&
                    TryParsePath(lambdaExpression.Body, out var path2) &&
                    path2 != null)
                {
                    path = $"{path1}.{path2}";
                    return true;
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
