// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.ExpressionVisitors;

using Aqua.EnumerableExtensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

[EditorBrowsable(EditorBrowsableState.Never)]
internal static class AsyncScalarQueryExpressionReWriter
{
    /// <summary>
    /// Type definition used in generic type filters.
    /// </summary>
    private sealed class ProbingType;

    private static readonly Dictionary<MethodInfo, MethodInfo> _methods = GetScalarQueryMethods();

    private static Dictionary<MethodInfo, MethodInfo> GetScalarQueryMethods()
    {
        static Type[] CreateGenericArguments(MethodInfo m) => Enumerable.Repeat(typeof(ProbingType), m.GetGenericArguments().Length).ToArray();

        static MethodInfo CreateClosedGenericTypeIfRequired(MethodInfo m) => m.IsGenericMethodDefinition ? m.MakeGenericMethod(CreateGenericArguments(m)) : m;

        static bool IsMatch(MethodInfo m1, MethodInfo m2)
        {
            m1 = CreateClosedGenericTypeIfRequired(m1);
            m2 = CreateClosedGenericTypeIfRequired(m2);
            var parameterList1 = m1.GetParameters();
            var parameterList2 = m2.GetParameters();
            return parameterList1.Length == parameterList2.Length - 1
                && typeof(Task<>).MakeGenericType(m1.ReturnType) == m2.ReturnType
                && parameterList1.Select(p => p.ParameterType).CollectionEquals(parameterList2.Select(p => p.ParameterType).Take(parameterList1.Length))
                && parameterList2.Last().ParameterType == typeof(CancellationToken);
        }

        var systemLinqMethods = typeof(Queryable)
            .GetTypeInfo()
            .DeclaredMethods
            .Where(x => x.IsPublic && !typeof(IQueryable).IsAssignableFrom(x.ReturnType))
            .GroupBy(x => x.Name);
        var entityFrameworkMethods = typeof(EntityFrameworkQueryableExtensions)
            .GetTypeInfo()
            .DeclaredMethods
            .GroupBy(x => x.Name);
        var list =
            from g1 in systemLinqMethods
            from g2 in entityFrameworkMethods
            where string.Equals($"{g1.Key}Async", g2.Key, StringComparison.Ordinal)
            from m1 in g1
            from m2 in g2
            where IsMatch(m1, m2)
            select (m1, m2);
        return list.ToDictionary(x => x.m1, x => x.m2);
    }

    /// <summary>
    /// Replaces scalar query expressions by the corresponding async EF Core version.
    /// </summary>
    internal static Expression ScalarQueryToAsyncExpression(this Expression expression, CancellationToken cancellation)
    {
        if (expression is MethodCallExpression methodCallExpression)
        {
            var methodDefinition = methodCallExpression.Method.GetGenericMethodDefinition();
            if (_methods.TryGetValue(methodDefinition, out var mappedMethodDefinition))
            {
                var mappedMethod = mappedMethodDefinition.MakeGenericMethod(methodCallExpression.Method.GetGenericArguments());
                var arguments = methodCallExpression.Arguments.Concat(new[] { Expression.Constant(cancellation) }).ToArray();
                expression = Expression.Call(methodCallExpression.Object, mappedMethod, arguments);
            }
        }

        return expression;
    }
}