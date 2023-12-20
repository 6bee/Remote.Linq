// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework.ExpressionExecution;

using Aqua.TypeExtensions;
using System;
using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Security;
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

    /// <summary>
    /// Type definition used in generic type filters.
    /// </summary>
    [SuppressMessage("Major Bug", "S3453:Classes should not have only \"private\" constructors", Justification = "For reflection only")]
    private sealed class TEntity
    {
        private TEntity()
        {
        }
    }

    private static readonly MethodInfo QueryableToListAsyncMethod = typeof(QueryableExtensions).GetMethodEx(
        nameof(QueryableExtensions.ToListAsync),
        new[] { typeof(TSource) },
        typeof(IQueryable<TSource>),
        typeof(CancellationToken));

    private static readonly MethodInfo DbContextSetMethod = typeof(DbContext).GetMethodEx(
        nameof(DbContext.Set),
        genericArguments: new[] { typeof(TEntity) });

    internal static Task ToListAsync(IQueryable source, CancellationToken cancellation)
    {
        source.AssertNotNull();
        var method = QueryableToListAsyncMethod.MakeGenericMethod(source.ElementType);
        var task = method.Invoke(null, new object[] { source, cancellation });
        return (Task)task!;
    }

    /// <summary>
    /// Returns the generic <see cref="DbSet{T}"/> for the type specified.
    /// </summary>
    /// <returns>Returns an instance of type <see cref="DbSet{T}"/>.</returns>
    [SecuritySafeCritical]
    internal static Func<Type, IQueryable> GetQueryableSetProvider(this DbContext dbContext)
        => new QueryableSetProvider(dbContext).GetQueryableSet;

    [SecuritySafeCritical]
    private sealed class QueryableSetProvider
    {
        private readonly DbContext _dbContext;

        [SecuritySafeCritical]
        public QueryableSetProvider(DbContext dbContext)
            => _dbContext = dbContext.CheckNotNull();

        [SecuritySafeCritical]
        public IQueryable GetQueryableSet(Type type)
        {
            var method = DbContextSetMethod.MakeGenericMethod(type);
            var set = method.Invoke(_dbContext, null);
            return (IQueryable)set!;
        }
    }
}