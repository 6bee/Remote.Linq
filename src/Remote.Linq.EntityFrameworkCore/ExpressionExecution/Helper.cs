// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.ExpressionExecution;

using Aqua.TypeExtensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

internal static class Helper
{
    /// <summary>
    /// Type definition used in generic type filters.
    /// </summary>
    [SuppressMessage("Major Bug", "S3453:Classes should not have only \"private\" constructors", Justification = "For reflection only")]
    internal sealed class TSource
    {
        private TSource()
        {
        }
    }

    private static readonly MethodInfo _entityFrameworkQueryableToListAsyncMethod = typeof(EntityFrameworkQueryableExtensions).GetMethodEx(
        nameof(EntityFrameworkQueryableExtensions.ToListAsync),
        [typeof(TSource)],
        typeof(IQueryable<TSource>),
        typeof(CancellationToken));

    private static readonly MethodInfo _executeAsAsyncStreamMethod = typeof(Helper).GetMethodEx(nameof(ExecuteAsAsyncStream));

    internal static Task ToListAsync(IQueryable source, CancellationToken cancellation)
    {
        source.AssertNotNull();
        var method = _entityFrameworkQueryableToListAsyncMethod.MakeGenericMethod(source.ElementType);
        var task = method.Invoke(null, [source, cancellation]);
        return (Task)task!;
    }

    public static IAsyncEnumerable<object?> ExecuteAsAsyncStreamWithCancellation(this IQueryable queryable, CancellationToken cancellation)
    {
        queryable.AssertNotNull();
        var method = _executeAsAsyncStreamMethod.MakeGenericMethod(queryable.ElementType);
        var asyncStream = method.Invoke(null, [queryable, cancellation]);
        return (IAsyncEnumerable<object?>)asyncStream!;
    }

    private static async IAsyncEnumerable<object?> ExecuteAsAsyncStream<T>(IQueryable<T> queryable, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var item in queryable.AsAsyncEnumerable().WithCancellation(cancellation).ConfigureAwait(false))
        {
            yield return item;
        }
    }
}