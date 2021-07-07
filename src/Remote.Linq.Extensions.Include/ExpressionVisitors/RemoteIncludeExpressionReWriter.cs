// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionVisitors
{
    using Aqua.TypeExtensions;
    using Remote.Linq;
    using Remote.Linq.Expressions;
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using SystemLinq = System.Linq.Expressions;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class RemoteIncludeExpressionReWriter
    {
        /// <summary>
        /// Replaces method <see cref="IncludeQueryableExtensions.Include{T, TProperty}(IQueryable{T}, SystemLinq.Expression{Func{T, TProperty}})"/>,
        /// <see cref="IncludeQueryableExtensions.ThenInclude{T, TPreviousProperty, TProperty}(IIncludableQueryable{T, TPreviousProperty}, SystemLinq.Expression{Func{TPreviousProperty, TProperty}})"/>, and
        /// <see cref="IncludeQueryableExtensions.ThenInclude{T, TPreviousProperty, TProperty}(IIncludableQueryable{T, TPreviousProperty}, SystemLinq.Expression{Func{TPreviousProperty, TProperty}})"/>
        /// by <see cref="IncludeQueryableExtensions.Include{T}(IQueryable{T}, string)"/>.
        /// </summary>
        public static Expression ReplaceIncludeQueryMethodsByStringInclude(this Expression expression, Func<Type, MethodInfo>? stringIncludeMethodInfoProvider = null)
        {
            var mapped = new QueryMethodMapper(stringIncludeMethodInfoProvider).Run(expression);
            var cleaned = new StackedQueryableCleaner().Run(mapped);
            return cleaned;
        }

        /// <summary>
        /// Replaces method <see cref="IncludeQueryableExtensions.ThenInclude{T, TPreviousProperty, TProperty}(IIncludableQueryable{T, TPreviousProperty}, SystemLinq.Expression{Func{TPreviousProperty, TProperty}})"/>
        /// and <see cref="IncludeQueryableExtensions.ThenInclude{T, TPreviousProperty, TProperty}(IIncludableQueryable{T, TPreviousProperty}, SystemLinq.Expression{Func{TPreviousProperty, TProperty}})"/>
        /// by <see cref="IncludeQueryableExtensions.Include{T, TProperty}(IQueryable{T}, SystemLinq.Expression{Func{T, TProperty}})"/> with sub-selects.
        /// </summary>
        public static Expression ReplaceThenIncludeQueryMethodsBySubSelects(this Expression expression)
        {
            var mapped = new QueryMethodMapper().Run(expression);
            var cleaned = new StackedQueryableCleaner(StackedQueryableCleaner.Strategy.SubSelect).Run(mapped);
            return cleaned;
        }

        private interface IStackedIncludableQueryable<out T>
        {
            Expression Expression { get; }

            string IncludePath { get; }

            Expression Parent { get; }
        }

        private sealed class StackedQueryableCleaner : RemoteExpressionVisitorBase
        {
            public enum Strategy
            {
                Eliminate,
                SubSelect,
            }

            private static readonly MethodInfo EliminateStackedIncludableQueryableMethodInfo = typeof(StackedQueryableCleaner).GetMethodEx(nameof(EliminateStackedIncludableQueryable));

            private static readonly MethodInfo SubselectStackedIncludableQueryableMethodInfo = typeof(StackedQueryableCleaner).GetMethodEx(nameof(SubselectStackedIncludableQueryable));

            private readonly Strategy _strategy;

            public StackedQueryableCleaner(Strategy strategy = Strategy.Eliminate)
                => _strategy = strategy;

            internal Expression Run(Expression expression) => Visit(expression);

            protected override Expression VisitConstant(ConstantExpression node)
            {
                if (node.Type.ToType().Implements(typeof(IStackedIncludableQueryable<>), out var args))
                {
                    var method = _strategy switch
                    {
                        Strategy.Eliminate => EliminateStackedIncludableQueryableMethodInfo,
                        Strategy.SubSelect => SubselectStackedIncludableQueryableMethodInfo,
                        _ => throw new NotSupportedException($"Strategy '{_strategy}' not supported."),
                    };

                    var m = method.MakeGenericMethod(args);
                    var expression = (Expression)m.Invoke(null, new[] { node.Value });
                    return Visit(expression);
                }

                return base.VisitConstant(node);
            }

            private static Expression EliminateStackedIncludableQueryable<T>(IStackedIncludableQueryable<T> source)
                => source.Expression;

            private static Expression SubselectStackedIncludableQueryable<T>(IStackedIncludableQueryable<T> source)
            {
                var lambda = SystemIncludeExpressionReWriter.BuildSubSelectNavigation<T>(source.IncludePath);

                var remoteLambda = SystemLinq.Expression.Quote(lambda)
                    .ToRemoteLinqExpression(ExpressionTranslator.NoMappingContext);

                var expression = new MethodCallExpression(
                    null,
                    IncludeQueryableExtensions.IncludeMethodInfo.MakeGenericMethod(lambda.Type.GetGenericArguments()),
                    new[] { source.Expression, remoteLambda });

                return expression;
            }
        }

        private sealed class QueryMethodMapper : RemoteExpressionVisitorBase
        {
            private static readonly MethodInfo IncludeMethodInfo = typeof(QueryMethodMapper).GetMethodEx(nameof(Include));

            private static readonly MethodInfo ThenIncludeAfterReferenceMethodInfo = typeof(QueryMethodMapper).GetMethodEx(nameof(ThenIncludeAfterReference));

            private static readonly MethodInfo ThenIncludeAfterEnumerableMethodInfo = typeof(QueryMethodMapper).GetMethodEx(nameof(ThenIncludeAfterEnumerable));

            private readonly Func<Type, MethodInfo> _stringIncludeMethodInfo;

            public QueryMethodMapper(Func<Type, MethodInfo>? stringIncludeMethodInfoProvider = null)
                => _stringIncludeMethodInfo = stringIncludeMethodInfoProvider ?? (type => IncludeQueryableExtensions.StringIncludeMethodInfo.MakeGenericMethod(type));

            internal Expression Run(Expression expression) => Visit(expression);

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (MapRemoteLinqToEntityFrameworkMethod(node.Method?.ToMethodInfo()) is MethodInfo mappedMethod)
                {
                    var arguments = node.Arguments.Select(Visit).ToArray();
                    var queryable = mappedMethod.Invoke(this, arguments);
                    return new ConstantExpression(queryable);
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

            private StackedIncludableQueryable<T, TProperty> Include<T, TProperty>(Expression queryable, Expression navigationPropertyPath)
                => IncludeCore<T, TProperty, TProperty>(queryable, navigationPropertyPath, false);

            private StackedIncludableQueryable<T, TProperty> ThenIncludeAfterReference<T, TPreviousProperty, TProperty>(Expression queryable, Expression navigationPropertyPath)
                => IncludeCore<T, TPreviousProperty, TProperty>(queryable, navigationPropertyPath, true);

            private StackedIncludableQueryable<T, TProperty> ThenIncludeAfterEnumerable<T, TPreviousProperty, TProperty>(Expression queryable, Expression navigationPropertyPath)
                => IncludeCore<T, TPreviousProperty, TProperty>(queryable, navigationPropertyPath, true);

            private StackedIncludableQueryable<T, TProperty> IncludeCore<T, TPreviousProperty, TProperty>(Expression queryableExpression, Expression navigationExpression, bool isNestedStatement)
            {
                queryableExpression.AssertNotNull(nameof(queryableExpression));
                navigationExpression.AssertNotNull(nameof(navigationExpression));

                var navigationPropertyPath = ToSystemLambdaExpression(navigationExpression);
                if (!TryParsePath(navigationPropertyPath.Body, out var path) || path is null)
                {
                    throw new ArgumentException("Invalid include path expression", nameof(navigationPropertyPath));
                }

                Expression source;
                if (isNestedStatement &&
                    queryableExpression is ConstantExpression constantExpression &&
                    constantExpression.Value is IStackedIncludableQueryable<T> preceding)
                {
                    source = preceding.Parent;
                    path = $"{preceding.IncludePath}.{path}";
                }
                else
                {
                    source = queryableExpression;
                }

                var stringIncludeExpression = new MethodCallExpression(
                    null,
                    _stringIncludeMethodInfo(typeof(T)),
                    new[] { source, new ConstantExpression(path) });

                return new StackedIncludableQueryable<T, TProperty>(
                    stringIncludeExpression,
                    source,
                    ////queryable.Provider,
                    path);
            }

            private static SystemLinq.LambdaExpression ToSystemLambdaExpression(Expression expression)
            {
                var systemExpression = expression.ToLinqExpression();
                if (systemExpression.NodeType == SystemLinq.ExpressionType.Quote)
                {
                    systemExpression = ((SystemLinq.UnaryExpression)systemExpression).Operand;
                }

                if (systemExpression is SystemLinq.LambdaExpression lambdaExpression)
                {
                    return lambdaExpression;
                }

                throw new RemoteLinqExpressionException(expression, $"Expected {nameof(LambdaExpression)} but got expression type {expression.NodeType} instead");
            }

            private static bool TryParsePath(SystemLinq.Expression expression, out string? path)
            {
                path = null;
                var expression1 = RemoveConvert(expression);
                if (expression1 is SystemLinq.MemberExpression memberExpression)
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
                else if (expression1 is SystemLinq.MethodCallExpression methodCallExpression)
                {
                    if (string.Equals(methodCallExpression.Method.Name, "Select", StringComparison.Ordinal) &&
                        methodCallExpression.Arguments.Count == 2 &&
                        TryParsePath(methodCallExpression.Arguments[0], out var path1) &&
                        path1 is not null &&
                        methodCallExpression.Arguments[1] is SystemLinq.LambdaExpression lambdaExpression &&
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

            private static SystemLinq.Expression RemoveConvert(SystemLinq.Expression expression)
            {
                while (expression.NodeType == SystemLinq.ExpressionType.Convert || expression.NodeType == SystemLinq.ExpressionType.ConvertChecked)
                {
                    expression = ((SystemLinq.UnaryExpression)expression).Operand;
                }

                return expression;
            }

            private sealed class StackedIncludableQueryable<T, TProperty> : /*IIncludableQueryable<T, TProperty>,*/ IStackedIncludableQueryable<T>
            {
                public StackedIncludableQueryable(Expression expression, Expression parent/*, IQueryProvider provider*/, string includePath)
                {
                    Expression = expression.CheckNotNull(nameof(expression));
                    Parent = parent.CheckNotNull(nameof(parent));
                    ////Provider = provider.CheckNotNull(nameof(provider));
                    IncludePath = includePath.CheckNotNullOrEmpty(nameof(includePath));
                }

                public string IncludePath { get; }

                public Expression Expression { get; }

                public Expression Parent { get; }

                ////public Type ElementType => typeof(T);

                ////public IQueryProvider Provider { get; }

                ////public IEnumerator<T> GetEnumerator()
                ////    => throw new NotSupportedException("This queryable serves as a placeholder and is not meant for execution");

                ////IEnumerator IEnumerable.GetEnumerator()
                ////    => GetEnumerator();
            }
        }
    }
}