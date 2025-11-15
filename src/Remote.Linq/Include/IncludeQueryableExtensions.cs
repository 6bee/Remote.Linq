// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Include;

using Aqua.TypeExtensions;
using Remote.Linq.Async;
using Remote.Linq.DynamicQuery;
using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class IncludeQueryableExtensions
{
    /// <summary>
    /// Type definition used in generic type filters.
    /// </summary>
    private sealed class T
    {
    }

    /// <summary>
    /// Type definition used in generic type filters.
    /// </summary>
    private sealed class TProperty
    {
    }

    /// <summary>
    /// Type definition used in generic type filters.
    /// </summary>
    private sealed class TPreviousProperty
    {
    }

    /// <summary>
    /// Gets the <see cref="MethodInfo"/> of the <see cref="Include{T}(IQueryable{T}, string)"/> method.
    /// </summary>
    public static readonly MethodInfo StringIncludeMethodInfo = typeof(IncludeQueryableExtensions).GetMethodEx(
        nameof(IncludeQueryableExtensions.Include),
        [typeof(T)],
        typeof(IQueryable<T>),
        typeof(string));

    /// <summary>
    /// Gets the <see cref="MethodInfo"/> of the <see cref="Include{T, TProperty}(IQueryable{T}, Expression{Func{T, TProperty}})"/> method.
    /// </summary>
    public static readonly MethodInfo IncludeMethodInfo = typeof(IncludeQueryableExtensions).GetMethodEx(
        nameof(IncludeQueryableExtensions.Include),
        [typeof(T), typeof(TProperty)],
        typeof(IQueryable<T>),
        typeof(Expression<Func<T, TProperty>>));

    /// <summary>
    /// Gets the <see cref="MethodInfo"/> of the
    /// <see cref="ThenInclude{T, TPreviousProperty, TProperty}(IIncludableQueryable{T, IEnumerable{TPreviousProperty}}, Expression{Func{TPreviousProperty, TProperty}})"/>
    /// method.
    /// </summary>
    public static readonly MethodInfo ThenIncludeAfterEnumerableMethodInfo = typeof(IncludeQueryableExtensions).GetMethodEx(
        nameof(IncludeQueryableExtensions.ThenInclude),
        [typeof(T), typeof(TPreviousProperty), typeof(TProperty)],
        typeof(IIncludableQueryable<T, IEnumerable<TPreviousProperty>>),
        typeof(Expression<Func<TPreviousProperty, TProperty>>));

    /// <summary>
    /// Gets the <see cref="MethodInfo"/> of the
    /// <see cref="ThenInclude{T, TPreviousProperty, TProperty}(IIncludableQueryable{T, TPreviousProperty}, Expression{Func{TPreviousProperty, TProperty}})"/>
    /// method.
    /// </summary>
    public static readonly MethodInfo ThenIncludeAfterReferenceMethodInfo = typeof(IncludeQueryableExtensions).GetMethodEx(
        nameof(IncludeQueryableExtensions.ThenInclude),
        [typeof(T), typeof(TPreviousProperty), typeof(TProperty)],
        typeof(IIncludableQueryable<T, TPreviousProperty>),
        typeof(Expression<Func<TPreviousProperty, TProperty>>));

    /// <summary>
    /// Specifies related entities to include in the query results.
    /// </summary>
    /// <typeparam name="T">The type of resource being queried.</typeparam>
    /// <param name="source">The source query.</param>
    /// <param name="navigationPropertyPath">A string of '.' separated navigation property names to be included.</param>
    /// <returns>A new query with the related data included.</returns>
    [QueryMarkerFunction]
    public static IQueryable<T> Include<T>(this IQueryable<T> source, string navigationPropertyPath)
    {
        source.AssertNotNull();
        navigationPropertyPath.AssertNotNullOrEmpty();

        return source.Provider is IRemoteLinqQueryProvider
            ? source.Provider.CreateQuery<T>(
                Expression.Call(
                    StringIncludeMethodInfo.MakeGenericMethod(typeof(T)),
                    source.Expression,
                    Expression.Constant(navigationPropertyPath)))
            : source;
    }

    /// <summary>
    /// Specifies related entities to include in the query results.
    /// </summary>
    /// <typeparam name="T">The type of resource being queried.</typeparam>
    /// <typeparam name="TProperty">The type of the related object to be included.</typeparam>
    /// <param name="source">The source query.</param>
    /// <param name="navigationPropertyPath">A lambda expression representing the navigation property to be included.</param>
    /// <returns>A new query with the related data included.</returns>
    [QueryMarkerFunction]
    public static IIncludableQueryable<T, TProperty> Include<T, TProperty>(this IQueryable<T> source, Expression<Func<T, TProperty>> navigationPropertyPath)
    {
        source.AssertNotNull();
        navigationPropertyPath.AssertNotNull();

        return new IncludableQueryable<T, TProperty>(
            source.Provider is IRemoteLinqQueryProvider
                ? source.Provider.CreateQuery<T>(
                    Expression.Call(
                        IncludeMethodInfo.MakeGenericMethod(typeof(T), typeof(TProperty)),
                        source.Expression,
                        Expression.Quote(navigationPropertyPath)))
                : source);
    }

    /// <summary>
    /// Specifies additional related data to be further included based on a related type that was just included.
    /// </summary>
    /// <typeparam name="T">The type of resource being queried.</typeparam>
    /// <typeparam name="TPreviousProperty">The type of the proeprty that was just included.</typeparam>
    /// <typeparam name="TProperty">The type of the related object to be included.</typeparam>
    /// <param name="source">The source query.</param>
    /// <param name="navigationPropertyPath">A lambda expression representing the navigation property to be included.</param>
    /// <returns>A new query with the related data included.</returns>
    [QueryMarkerFunction]
    public static IIncludableQueryable<T, TProperty> ThenInclude<T, TPreviousProperty, TProperty>(
        this IIncludableQueryable<T, TPreviousProperty> source,
        Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
    {
        source.AssertNotNull();
        navigationPropertyPath.AssertNotNull();

        return new IncludableQueryable<T, TProperty>(
            source.Provider is IRemoteLinqQueryProvider
                ? source.Provider.CreateQuery<T>(
                    Expression.Call(
                        ThenIncludeAfterReferenceMethodInfo.MakeGenericMethod(typeof(T), typeof(TPreviousProperty), typeof(TProperty)),
                        source.Expression,
                        Expression.Quote(navigationPropertyPath)))
                : source);
    }

    /// <summary>
    /// Specifies additional related data to be further included based on a related type that was just included.
    /// </summary>
    /// <typeparam name="T">The type of resource being queried.</typeparam>
    /// <typeparam name="TPreviousProperty">The type of the proeprty that was just included.</typeparam>
    /// <typeparam name="TProperty">The type of the related object to be included.</typeparam>
    /// <param name="source">The source query.</param>
    /// <param name="navigationPropertyPath">A lambda expression representing the navigation property to be included.</param>
    /// <returns>A new query with the related data included.</returns>
    [QueryMarkerFunction]
    public static IIncludableQueryable<T, TProperty> ThenInclude<T, TPreviousProperty, TProperty>(
        this IIncludableQueryable<T, IEnumerable<TPreviousProperty>> source,
        Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
    {
        source.AssertNotNull();
        navigationPropertyPath.AssertNotNull();

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
            => _queryable = queryable.CheckNotNull();

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
            => ((IEnumerable)_queryable).GetEnumerator();
    }
}