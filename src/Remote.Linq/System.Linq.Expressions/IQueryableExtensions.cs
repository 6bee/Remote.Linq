// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Reflection;

namespace System.Linq.Expressions
{
    public static class IQueryableExtensions
    {
        private static readonly MethodInfo _lambdaExpressionMethodInfo = typeof(Expression)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(m =>
                m.Name == "Lambda" &&
                m.IsGenericMethod &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[1].ParameterType == typeof(ParameterExpression[]));

        private static TResultingQueriable Call<TSource, TQueriable, TResultingQueriable>(this TQueriable queryable, LambdaExpression lambdaExpression, string methodName)
        {
            var exp = lambdaExpression.Body;
            var resultType = exp.Type;
            var funcType = typeof(Func<,>).MakeGenericType(typeof(TSource), resultType);
            var lambdaExpressionMethodInfo = _lambdaExpressionMethodInfo.MakeGenericMethod(funcType);

            var funcExpression = lambdaExpressionMethodInfo.Invoke(null, new object[] { exp, lambdaExpression.Parameters.ToArray() });

            var method = typeof(TQueriable).GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod(resultType);
            var result = method.Invoke(queryable, new object[] { funcExpression });

            return (TResultingQueriable)result;
        }

        public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> queryable, LambdaExpression lambdaExpression)
        {
            return queryable.Call<TSource, IQueryable<TSource>, IOrderedQueryable<TSource>>(lambdaExpression, "OrderBy");
        }

        public static IOrderedQueryable<TEntity> OrderByDescending<TEntity>(this IQueryable<TEntity> queryable, LambdaExpression lambdaExpression)
        {
            return queryable.Call<TEntity, IQueryable<TEntity>, IOrderedQueryable<TEntity>>(lambdaExpression, "OrderByDescending");
        }

        public static IOrderedQueryable<TEntity> ThenBy<TEntity>(this IOrderedQueryable<TEntity> queryable, LambdaExpression lambdaExpression)
        {
            return queryable.Call<TEntity, IOrderedQueryable<TEntity>, IOrderedQueryable<TEntity>>(lambdaExpression, "ThenBy");
        }

        public static IOrderedQueryable<TEntity> ThenByDescending<TEntity>(this IOrderedQueryable<TEntity> queryable, LambdaExpression lambdaExpression)
        {
            return queryable.Call<TEntity, IOrderedQueryable<TEntity>, IOrderedQueryable<TEntity>>(lambdaExpression, "ThenByDescending");
        }
    }
}
