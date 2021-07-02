// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.ExpressionExecution
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Threading;
    using System.Threading.Tasks;
    using static MethodInfos;

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

        private static readonly MethodInfo _entityFrameworkQueryableToListAsyncMethod = GetMethod(
            typeof(EntityFrameworkQueryableExtensions),
            nameof(EntityFrameworkQueryableExtensions.ToListAsync),
            new[] { typeof(TSource) },
            typeof(IQueryable<TSource>),
            typeof(CancellationToken));

        private static readonly MethodInfo _dbContextSetMethod = GetMethod(
            typeof(DbContext),
            nameof(DbContext.Set),
            genericArguments: new[] { typeof(TEntity) });

        private static readonly MethodInfo _executeAsAsyncStreamMethod =
            GetMethod(typeof(Helper), nameof(ExecuteAsAsyncStream));

        internal static Task ToListAsync(IQueryable source, CancellationToken cancellation)
        {
            source.AssertNotNull(nameof(source));
            var method = _entityFrameworkQueryableToListAsyncMethod.MakeGenericMethod(source.ElementType);
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
                => _dbContext = dbContext.CheckNotNull(nameof(dbContext));

            [SecuritySafeCritical]
            public IQueryable GetQueryableSet(Type type)
            {
                var method = _dbContextSetMethod.MakeGenericMethod(type);
                var set = method.Invoke(_dbContext, null);
                return (IQueryable)set!;
            }
        }

        public static IAsyncEnumerable<object?> ExecuteAsAsyncStreamWithCancellation(this IQueryable queryable, CancellationToken cancellation)
        {
            queryable.AssertNotNull(nameof(queryable));
            var method = _executeAsAsyncStreamMethod.MakeGenericMethod(queryable.ElementType);
            var asyncStream = method.Invoke(null, new object[] { queryable, cancellation });
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
}