// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework.ExpressionExecution;

using Aqua.TypeExtensions;
using System.Data.Entity;
using System.Reflection;

internal static class Helper
{
    /// <summary>
    /// Type definition used in generic type filters.
    /// </summary>
    private sealed class TSource;

    private static readonly MethodInfo _queryableToListAsyncMethod = typeof(QueryableExtensions).GetMethodEx(
        nameof(QueryableExtensions.ToListAsync),
        [typeof(TSource)],
        typeof(IQueryable<TSource>),
        typeof(CancellationToken));

    internal static Task ToListAsync(IQueryable source, CancellationToken cancellation)
    {
        source.AssertNotNull();
        var method = _queryableToListAsyncMethod.MakeGenericMethod(source.ElementType);
        var task = method.Invoke(null, [source, cancellation]);
        return (Task)task!;
    }
}