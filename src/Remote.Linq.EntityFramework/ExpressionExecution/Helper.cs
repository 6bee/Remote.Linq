// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework.ExpressionExecution;

using Aqua.TypeExtensions;
using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

internal static class Helper
{
    /// <summary>
    /// Type definition used in generic type filters.
    /// </summary>
    [SuppressMessage("Major Bug", "S3453:Classes should not have only \"private\" constructors", Justification = "For reflection only")]
    private sealed class TSource
    {
        private TSource()
        {
        }
    }

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