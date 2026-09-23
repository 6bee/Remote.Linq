// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1649 // File name should match first type name

#nullable enable

namespace Remote.Linq.Tests.TestSupport;

using Remote.Linq.DynamicQuery;
using System.Runtime.CompilerServices;
using RemoteLinq = Remote.Linq.Expressions;
using SystemLinq = System.Linq.Expressions;

/// <summary>
/// Records invocations of a deterministic synchronous data provider.
/// </summary>
/// <typeparam name="TSource">Type of source data values.</typeparam>
internal sealed class RecordingSyncProvider<TSource>(TSource result)
{
    /// <summary>
    /// Gets the number of invocations of the data provider.
    /// </summary>
    public int InvocationCount { get; private set; }

    /// <summary>
    /// Gets the last expression passed to the data provider.
    /// </summary>
    public RemoteLinq.Expression? LastExpression { get; private set; }

    /// <summary>
    /// Gets the deterministic result returned by the data provider.
    /// </summary>
    public TSource Result { get; } = result;

    /// <summary>
    /// Gets the data provider delegate.
    /// </summary>
    public Func<RemoteLinq.Expression, TSource> DataProvider => expression =>
    {
        InvocationCount++;
        LastExpression = expression;
        return Result;
    };
}

/// <summary>
/// Records invocations of a deterministic asynchronous data provider.
/// </summary>
/// <typeparam name="TSource">Type of source data values.</typeparam>
internal sealed class RecordingAsyncProvider<TSource>(TSource result)
{
    /// <summary>
    /// Gets the number of invocations of the data provider.
    /// </summary>
    public int InvocationCount { get; private set; }

    /// <summary>
    /// Gets the last expression passed to the data provider.
    /// </summary>
    public RemoteLinq.Expression? LastExpression { get; private set; }

    /// <summary>
    /// Gets the last cancellation token passed to the data provider.
    /// </summary>
    public CancellationToken LastToken { get; private set; }

    /// <summary>
    /// Gets the deterministic result returned by the data provider.
    /// </summary>
    public TSource Result { get; } = result;

    /// <summary>
    /// Gets the data provider delegate.
    /// </summary>
    public Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource>> DataProvider => (expression, token) =>
    {
        InvocationCount++;
        LastExpression = expression;
        LastToken = token;
        return new ValueTask<TSource>(Result);
    };
}

/// <summary>
/// Records invocations of a deterministic asynchronous stream data provider.
/// </summary>
/// <typeparam name="TSource">Type of source data values.</typeparam>
internal sealed class RecordingAsyncStreamProvider<TSource>(IEnumerable<TSource> values)
{
    /// <summary>
    /// Gets the number of invocations of the data provider.
    /// </summary>
    public int InvocationCount { get; private set; }

    /// <summary>
    /// Gets the last expression passed to the data provider.
    /// </summary>
    public RemoteLinq.Expression? LastExpression { get; private set; }

    /// <summary>
    /// Gets the last cancellation token passed to the data provider.
    /// </summary>
    public CancellationToken LastToken { get; private set; }

    /// <summary>
    /// Gets the deterministic values yielded by the data provider.
    /// </summary>
    public IReadOnlyList<TSource> Values { get; } = [.. values];

    /// <summary>
    /// Gets the non-token async enumerable factory delegate.
    /// </summary>
    public Func<RemoteLinq.Expression, IAsyncEnumerable<TSource>> DataProvider =>
        expression => CreateAsyncEnumerable(expression, CancellationToken.None);

    /// <summary>
    /// Gets the token async enumerable factory delegate.
    /// </summary>
    public Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<TSource>> DataProviderWithToken => CreateAsyncEnumerable;

    private async IAsyncEnumerable<TSource> CreateAsyncEnumerable(RemoteLinq.Expression expression, [EnumeratorCancellation] CancellationToken token = default)
    {
        InvocationCount++;
        LastExpression = expression;
        LastToken = token;
        foreach (var value in Values)
        {
            yield return value;
        }
    }
}

/// <summary>
/// Maps query results, recording the expression and returning either a configured result or the source value.
/// </summary>
/// <typeparam name="TSource">Type of source data values.</typeparam>
internal sealed class PassthroughQueryResultMapper<TSource> : IQueryResultMapper<TSource>
{
    /// <summary>
    /// Gets the number of invocations of the mapper.
    /// </summary>
    public int InvocationCount { get; private set; }

    /// <summary>
    /// Gets the last expression passed to the mapper.
    /// </summary>
    public SystemLinq.Expression? LastExpression { get; private set; }

    /// <summary>
    /// Gets or sets a configured result. If unset, the source value itself is returned.
    /// </summary>
    public object? ConfiguredResult { get; set; }

    /// <inheritdoc/>
    public TResult? MapResult<TResult>(TSource? source, SystemLinq.Expression expression)
    {
        InvocationCount++;
        LastExpression = expression;
        return ConfiguredResult is null ? (TResult?)(object?)source : (TResult?)ConfiguredResult;
    }
}

/// <summary>
/// Maps query results asynchronously, recording the expression and cancellation token and
/// returning either a configured result or the source value.
/// </summary>
/// <typeparam name="TSource">Type of source data values.</typeparam>
internal sealed class PassthroughAsyncQueryResultMapper<TSource> : IAsyncQueryResultMapper<TSource>
{
    /// <summary>
    /// Gets the number of invocations of the mapper.
    /// </summary>
    public int InvocationCount { get; private set; }

    /// <summary>
    /// Gets the last expression passed to the mapper.
    /// </summary>
    public SystemLinq.Expression? LastExpression { get; private set; }

    /// <summary>
    /// Gets the last cancellation token passed to the mapper.
    /// </summary>
    public CancellationToken LastToken { get; private set; }

    /// <summary>
    /// Gets or sets a configured result. If unset, the source value itself is returned.
    /// </summary>
    public object? ConfiguredResult { get; set; }

    /// <inheritdoc/>
    public ValueTask<TResult> MapResultAsync<TResult>(TSource? source, SystemLinq.Expression expression, CancellationToken cancellation = default)
    {
        InvocationCount++;
        LastExpression = expression;
        LastToken = cancellation;
        TResult? result = ConfiguredResult is null ? (TResult?)(object?)source : (TResult?)ConfiguredResult;
        return new ValueTask<TResult>(result!);
    }
}
