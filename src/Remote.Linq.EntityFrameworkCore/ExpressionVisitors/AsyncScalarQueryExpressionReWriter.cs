// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.ExpressionVisitors
{
    using Aqua.Extensions;
    using Microsoft.EntityFrameworkCore;
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
        [SuppressMessage("Major Bug", "S3453:Classes should not have only \"private\" constructors", Justification = "For reflection only")]
        private sealed class ProbingType
        {
            private ProbingType()
            {
            }
        }

        private static readonly Dictionary<MethodInfo, MethodInfo> _methods = GetScalarQueryMethods().ToDictionary(x => x.Item1, x => x.Item2);

        private static IEnumerable<Tuple<MethodInfo, MethodInfo>> GetScalarQueryMethods()
        {
            static Type[] CreateGenericArguments(MethodInfo m) => Enumerable.Range(0, m.GetGenericArguments().Length)
                .Select(x => typeof(ProbingType))
                .ToArray();

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
                select Tuple.Create(m1, m2);
            return list;
        }

        /// <summary>
        /// Replaces scalar query expressions by the corresponding async EF Core version.
        /// </summary>
        internal static Expression ScalarQueryToAsyncExpression(this Expression expression, CancellationToken cancellation)
        {
            if (expression is MethodCallExpression methodCallExpression)
            {
                var methodDefinition = methodCallExpression.Method.GetGenericMethodDefinition();
                if (_methods.TryGetValue(methodDefinition, out MethodInfo mappedMethodDefinition))
                {
                    var mappedMethod = mappedMethodDefinition.MakeGenericMethod(methodCallExpression.Method.GetGenericArguments());
                    var arguments = methodCallExpression.Arguments.Concat(new[] { Expression.Constant(cancellation) }).ToArray();
                    expression = Expression.Call(methodCallExpression.Object, mappedMethod, arguments);
                }
            }

            return expression;
        }
    }
}
