// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Remote.Linq.DynamicQuery;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class IncludeQueryableExtensions
    {
        /// <summary>
        /// Type definition used in generic type filters.
        /// </summary>
        [SuppressMessage("Major Bug", "S3453:Classes should not have only \"private\" constructors", Justification = "For reflection only")]
        private sealed class T
        {
            private T()
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

        public static readonly MethodInfo StringIncludeMethodInfo = MethodInfos.GetMethod(
            typeof(IncludeQueryableExtensions),
            nameof(IncludeQueryableExtensions.Include),
            new[] { typeof(T) },
            typeof(IQueryable<T>),
            typeof(string));

        public static readonly MethodInfo IncludeMethodInfo = MethodInfos.GetMethod(
            typeof(IncludeQueryableExtensions),
            nameof(IncludeQueryableExtensions.Include),
            new[] { typeof(T), typeof(TProperty) },
            typeof(IQueryable<T>),
            typeof(Expression<Func<T, TProperty>>));

        public static readonly MethodInfo ThenIncludeAfterEnumerableMethodInfo = MethodInfos.GetMethod(
            typeof(IncludeQueryableExtensions),
            nameof(IncludeQueryableExtensions.ThenInclude),
            new[] { typeof(T), typeof(TPreviousProperty), typeof(TProperty) },
            typeof(IIncludableQueryable<T, IEnumerable<TPreviousProperty>>),
            typeof(Expression<Func<TPreviousProperty, TProperty>>));

        public static readonly MethodInfo ThenIncludeAfterReferenceMethodInfo = MethodInfos.GetMethod(
            typeof(IncludeQueryableExtensions),
            nameof(IncludeQueryableExtensions.ThenInclude),
            new[] { typeof(T), typeof(TPreviousProperty), typeof(TProperty) },
            typeof(IIncludableQueryable<T, TPreviousProperty>),
            typeof(Expression<Func<TPreviousProperty, TProperty>>));

        [QueryMarkerFunction]
        public static IQueryable<T> Include<T>(this IQueryable<T> source, string navigationPropertyPath)
        {
            source.AssertNotNull(nameof(source));
            navigationPropertyPath.AssertNotNullOrEmpty(nameof(navigationPropertyPath));

            return source.Provider is IRemoteLinqQueryProvider
                ? source.Provider.CreateQuery<T>(
                    Expression.Call(
                        StringIncludeMethodInfo.MakeGenericMethod(typeof(T)),
                        source.Expression,
                        Expression.Constant(navigationPropertyPath)))
                : source;
        }

        [QueryMarkerFunction]
        public static IIncludableQueryable<T, TProperty> Include<T, TProperty>(this IQueryable<T> source, Expression<Func<T, TProperty>> navigationPropertyPath)
        {
            source.AssertNotNull(nameof(source));
            navigationPropertyPath.AssertNotNull(nameof(navigationPropertyPath));

            return new IncludableQueryable<T, TProperty>(
                source.Provider is IRemoteLinqQueryProvider
                    ? source.Provider.CreateQuery<T>(
                        Expression.Call(
                            IncludeMethodInfo.MakeGenericMethod(typeof(T), typeof(TProperty)),
                            source.Expression,
                            Expression.Quote(navigationPropertyPath)))
                    : source);
        }

        [QueryMarkerFunction]
        public static IIncludableQueryable<T, TProperty> ThenInclude<T, TPreviousProperty, TProperty>(
            this IIncludableQueryable<T, TPreviousProperty> source,
            Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
        {
            source.AssertNotNull(nameof(source));
            navigationPropertyPath.AssertNotNull(nameof(navigationPropertyPath));

            return new IncludableQueryable<T, TProperty>(
                source.Provider is IRemoteLinqQueryProvider
                    ? source.Provider.CreateQuery<T>(
                        Expression.Call(
                            ThenIncludeAfterReferenceMethodInfo.MakeGenericMethod(typeof(T), typeof(TPreviousProperty), typeof(TProperty)),
                            source.Expression,
                            Expression.Quote(navigationPropertyPath)))
                    : source);
        }

        [QueryMarkerFunction]
        public static IIncludableQueryable<T, TProperty> ThenInclude<T, TPreviousProperty, TProperty>(
            this IIncludableQueryable<T, IEnumerable<TPreviousProperty>> source,
            Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
        {
            source.AssertNotNull(nameof(source));
            navigationPropertyPath.AssertNotNull(nameof(navigationPropertyPath));

            return new IncludableQueryable<T, TProperty>(
                source.Provider is IRemoteLinqQueryProvider
                    ? source.Provider.CreateQuery<T>(
                        Expression.Call(
                            ThenIncludeAfterEnumerableMethodInfo.MakeGenericMethod(typeof(T), typeof(TPreviousProperty), typeof(TProperty)),
                            source.Expression,
                            Expression.Quote(navigationPropertyPath)))
                    : source);
        }

        private sealed class IncludableQueryable<T, TProperty> : IIncludableQueryable<T, TProperty>, IAsyncEnumerable<T>
        {
            private readonly IQueryable<T> _queryable;

            internal IncludableQueryable(IQueryable<T> queryable)
                => _queryable = queryable.CheckNotNull(nameof(queryable));

            public Expression Expression
                => _queryable.Expression;

            public Type ElementType
                => _queryable.ElementType;

            public IQueryProvider Provider
                => _queryable.Provider;

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
                => _queryable.AsAsyncEnumerable().GetAsyncEnumerator(cancellationToken);

            public IEnumerator<T> GetEnumerator()
                => _queryable.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator()
                => GetEnumerator();
        }
    }
}