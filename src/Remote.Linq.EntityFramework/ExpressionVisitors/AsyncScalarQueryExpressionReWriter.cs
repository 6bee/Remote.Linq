// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework.ExpressionVisitors;

using Aqua.EnumerableExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

[EditorBrowsable(EditorBrowsableState.Never)]
internal static class AsyncScalarQueryExpressionReWriter
{
    /// <summary>
    /// Type definition used in generic type filters.
    /// </summary>
    [SuppressMessage("Major Bug", "S3453:Classes should not have only \"private\" constructors", Justification = "For reflection only")]
    private sealed class ProbingType
    {
        private ProbingType()
        {
        }
    }

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
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(x => !typeof(IQueryable).IsAssignableFrom(x.ReturnType))
            .GroupBy(x => x.Name);
        var entityFrameworkMethods = typeof(System.Data.Entity.QueryableExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
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
        if (expression is MethodCallExpression methodCallExpression && methodCallExpression.Method.IsGenericMethod)
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