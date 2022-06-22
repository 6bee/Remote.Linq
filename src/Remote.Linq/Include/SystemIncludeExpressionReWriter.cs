// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Include
{
    using Aqua.TypeExtensions;
    using Remote.Linq.ExpressionExecution;
    using Remote.Linq.ExpressionVisitors;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class SystemIncludeExpressionReWriter
    {
        /// <summary>
        /// Replaces method <see cref="IncludeQueryableExtensions.Include{T, TProperty}(IQueryable{T}, Expression{Func{T, TProperty}})"/>,
        /// <see cref="IncludeQueryableExtensions.ThenInclude{T, TPreviousProperty, TProperty}(IIncludableQueryable{T, TPreviousProperty}, Expression{Func{TPreviousProperty, TProperty}})"/>, and
        /// <see cref="IncludeQueryableExtensions.ThenInclude{T, TPreviousProperty, TProperty}(IIncludableQueryable{T, TPreviousProperty}, Expression{Func{TPreviousProperty, TProperty}})"/>
        /// by <see cref="IncludeQueryableExtensions.Include{T}(IQueryable{T}, string)"/>.
        /// </summary>
        public static Expression ReplaceIncludeQueryMethodsByStringInclude(this Expression expression, Func<Type, MethodInfo>? stringIncludeMethodInfoProvider = null)
        {
            var mapped = new QueryMethodMapper(stringIncludeMethodInfoProvider).Run(expression);
            var cleaned = new StackedQueryableCleaner().Run(mapped);
            return cleaned;
        }

        /// <summary>
        /// Replaces method <see cref="IncludeQueryableExtensions.ThenInclude{T, TPreviousProperty, TProperty}(IIncludableQueryable{T, TPreviousProperty}, Expression{Func{TPreviousProperty, TProperty}})"/> and
        /// <see cref="IncludeQueryableExtensions.ThenInclude{T, TPreviousProperty, TProperty}(IIncludableQueryable{T, TPreviousProperty}, Expression{Func{TPreviousProperty, TProperty}})"/>
        /// by <see cref="IncludeQueryableExtensions.Include{T, TProperty}(IQueryable{T}, Expression{Func{T, TProperty}})"/> with sub-selects.
        /// </summary>
        public static Expression ReplaceThenIncludeQueryMethodsBySubSelects(this Expression expression)
        {
            var mapped = new QueryMethodMapper().Run(expression);
            var cleaned = new StackedQueryableCleaner(StackedQueryableCleaner.Strategy.SubSelect).Run(mapped);
            return cleaned;
        }

        internal static LambdaExpression BuildSubSelectNavigation<T>(string path)
        {
            path.AssertNotNullOrEmpty(nameof(path));

            var segments = path.Split('.');

            var x = Expression.Parameter(typeof(T), "x");

            var navigation = Subselect(x, segments);

            static Expression Subselect(Expression expression, IEnumerable<string> segments, int i = 0)
            {
                var segment = segments.FirstOrDefault();
                if (segment is null)
                {
                    return expression;
                }

                const BindingFlags Any = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
                var element = expression.Type;
                var p = Aqua.TypeSystem.TypeHelper.GetElementType(element).GetProperty(segment, Any) !;
                if (element.Implements(typeof(IEnumerable<>)) && element != typeof(string))
                {
                    var x = Expression.Parameter(p.DeclaringType!, $"x{++i}");
                    var selector = Expression.MakeMemberAccess(x, p);
                    var navigation = Subselect(selector, segments.Skip(1), i);
                    return Expression.Call(
                        MethodInfos.Enumerable.Select.MakeGenericMethod(x.Type, navigation.Type),
                        expression,
                        Expression.Lambda(navigation, x));
                }
                else
                {
                    var navigation = Expression.MakeMemberAccess(expression, p);
                    return Subselect(navigation, segments.Skip(1), i);
                }
            }

            var lambda = Expression.Lambda(
                typeof(Func<,>).MakeGenericType(x.Type, navigation.Type),
                navigation,
                x);

            return lambda;
        }

        private interface IStackedIncludableQueryable<out T> : IQueryable<T>
        {
            string IncludePath { get; }

            Expression Parent { get; }
        }

        private sealed class StackedQueryableCleaner : SystemExpressionVisitorBase
        {
            public enum Strategy
            {
                Eliminate,
                SubSelect,
            }

            private static readonly MethodInfo EliminateStackedIncludableQueryableMethodInfo =
                typeof(StackedQueryableCleaner).GetMethodEx(nameof(EliminateStackedIncludableQueryable));

            private static readonly MethodInfo SubselectStackedIncludableQueryableMethodInfo =
                typeof(StackedQueryableCleaner).GetMethodEx(nameof(SubselectStackedIncludableQueryable));

            private readonly Strategy _strategy;

            public StackedQueryableCleaner(Strategy strategy = Strategy.Eliminate)
                => _strategy = strategy;

            internal Expression Run(Expression expression) => Visit(expression);

            protected override Expression VisitConstant(ConstantExpression node)
            {
                if (node.Type.Implements(typeof(IStackedIncludableQueryable<>), out var args) && node.Value is IQueryable queryable)
                {
                    var method = _strategy switch
                    {
                        Strategy.Eliminate => EliminateStackedIncludableQueryableMethodInfo,
                        Strategy.SubSelect => SubselectStackedIncludableQueryableMethodInfo,
                        _ => throw new NotSupportedException($"Strategy '{_strategy}' not supported."),
                    };

                    var m = method.MakeGenericMethod(args);
                    var expression = (Expression)m.Invoke(null, new[] { queryable }) !;
                    return Visit(expression);
                }

                return base.VisitConstant(node);
            }

            private static Expression EliminateStackedIncludableQueryable<T>(IStackedIncludableQueryable<T> source)
                => source.Expression;

            private static Expression SubselectStackedIncludableQueryable<T>(IStackedIncludableQueryable<T> source)
            {
                var lambda = BuildSubSelectNavigation<T>(source.IncludePath);
                var expression = Expression.Call(
                    IncludeQueryableExtensions.IncludeMethodInfo.MakeGenericMethod(lambda.Type.GetGenericArguments()),
                    source.Expression,
                    Expression.Quote(lambda));
                return expression;
            }
        }

        private sealed class QueryMethodMapper : SystemExpressionVisitorBase
        {
            private static readonly MethodInfo IncludeMethodInfo =
                typeof(QueryMethodMapper).GetMethodEx(nameof(Include));

            private static readonly MethodInfo ThenIncludeAfterReferenceMethodInfo =
                typeof(QueryMethodMapper).GetMethodEx(nameof(ThenIncludeAfterReference));

            private static readonly MethodInfo ThenIncludeAfterEnumerableMethodInfo =
                typeof(QueryMethodMapper).GetMethodEx(nameof(ThenIncludeAfterEnumerable));

            private readonly Func<Type, MethodInfo> _stringIncludeMethodInfo;

            public QueryMethodMapper(Func<Type, MethodInfo>? stringIncludeMethodInfoProvider = null)
                => _stringIncludeMethodInfo = stringIncludeMethodInfoProvider ?? (type => IncludeQueryableExtensions.StringIncludeMethodInfo.MakeGenericMethod(type));

            internal Expression Run(Expression expression) => Visit(expression);

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (MapRemoteLinqToEntityFrameworkMethod(node.Method) is MethodInfo mappedMethod)
                {
                    var arguments = node.Arguments?.Select<Expression, Expression>(Visit).ToArray();
                    var method = Expression.Call(Expression.Constant(this), mappedMethod, arguments);
                    var queryable = method.CompileAndInvokeExpression();
                    return Expression.Constant(queryable);
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
                            return IncludeMethodInfo;
                        }

                        if (method == IncludeQueryableExtensions.ThenIncludeAfterEnumerableMethodInfo)
                        {
                            return ThenIncludeAfterEnumerableMethodInfo;
                        }

                        if (method == IncludeQueryableExtensions.ThenIncludeAfterReferenceMethodInfo)
                        {
                            return ThenIncludeAfterReferenceMethodInfo;
                        }

                        return null;
                    }
                }
            }

            private IIncludableQueryable<T, TProperty> Include<T, TProperty>(IQueryable<T> queryable, Expression<Func<T, TProperty>> navigationPropertyPath)
                => IncludeCore(queryable, navigationPropertyPath, false);

            private IIncludableQueryable<T, TProperty> ThenIncludeAfterReference<T, TPreviousProperty, TProperty>(
                IIncludableQueryable<T, TPreviousProperty> queryable,
                Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
                => IncludeCore(queryable, navigationPropertyPath, true);

            private IIncludableQueryable<T, TProperty> ThenIncludeAfterEnumerable<T, TPreviousProperty, TProperty>(
                IIncludableQueryable<T, IEnumerable<TPreviousProperty>> queryable,
                Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
                => IncludeCore(queryable, navigationPropertyPath, true);

            private IIncludableQueryable<T, TProperty> IncludeCore<T, TPreviousProperty, TProperty>(
                IQueryable<T> queryable,
                Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath,
                bool isNestedStatement)
            {
                queryable.AssertNotNull(nameof(queryable));
                navigationPropertyPath.AssertNotNull(nameof(navigationPropertyPath));

                if (!TryParsePath(navigationPropertyPath.Body, out var path) || path is null)
                {
                    throw new ArgumentException("Invalid include path expression", nameof(navigationPropertyPath));
                }

                Expression source;
                if (isNestedStatement && queryable is IStackedIncludableQueryable<T> preceding)
                {
                    source = preceding.Parent;
                    path = $"{preceding.IncludePath}.{path}";
                }
                else
                {
                    source = Expression.Constant(queryable);
                }

                var stringIncludeExpression = Expression.Call(
                    _stringIncludeMethodInfo(typeof(T)),
                    source,
                    Expression.Constant(path));

                return new StackedIncludableQueryable<T, TProperty>(
                    stringIncludeExpression,
                    source,
                    path);
            }

            private static bool TryParsePath(Expression expression, out string? path)
            {
                path = null;
                var expression1 = RemoveConvert(expression);
                if (expression1 is MemberExpression memberExpression)
                {
                    if (memberExpression.Expression is null)
                    {
                        return false;
                    }

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
                        path1 is not null &&
                        methodCallExpression.Arguments[1] is LambdaExpression lambdaExpression &&
                        TryParsePath(lambdaExpression.Body, out var path2) &&
                        path2 is not null)
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

            private sealed class StackedIncludableQueryable<T, TProperty> : IIncludableQueryable<T, TProperty>, IStackedIncludableQueryable<T>
            {
                public StackedIncludableQueryable(Expression expression, Expression parent, string includePath)
                {
                    Expression = expression.CheckNotNull(nameof(expression));
                    Parent = parent.CheckNotNull(nameof(parent));
                    IncludePath = includePath.CheckNotNullOrEmpty(nameof(includePath));
                }

                public string IncludePath { get; }

                public Expression Expression { get; }

                public Expression Parent { get; }

                public Type ElementType => throw NotSupportedException;

                public IQueryProvider Provider => throw NotSupportedException;

                public IEnumerator<T> GetEnumerator() => throw NotSupportedException;

                IEnumerator IEnumerable.GetEnumerator() => throw NotSupportedException;

                private NotSupportedException NotSupportedException => new NotSupportedException("This queryable serves as a placeholder and is not meant for execution");
            }
        }
    }
}