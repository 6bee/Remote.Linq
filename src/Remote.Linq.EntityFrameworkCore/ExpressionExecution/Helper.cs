// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.ExpressionExecution
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Security;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class Helper
    {
        private const BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static;
        private const BindingFlags PrivateStatic = BindingFlags.NonPublic | BindingFlags.Static;

        private static readonly MethodInfo _toListAsync = typeof(EntityFrameworkQueryableExtensions)
            .GetMethods(PublicStatic)
            .Where(m => string.Equals(m.Name, nameof(EntityFrameworkQueryableExtensions.ToListAsync), StringComparison.Ordinal))
            .Where(m => m.IsGenericMethodDefinition && m.GetParameters().Length == 2)
            .Single();

        private static readonly MethodInfo _dbSetGetter = typeof(DbContext)
            .GetMethods()
            .Where(x => string.Equals(x.Name, nameof(DbContext.Set), StringComparison.Ordinal))
            .Where(x => x.IsGenericMethod)
            .Single(x => x.GetGenericArguments().Length == 1 && x.GetParameters().Length == 0);

        private static readonly MethodInfo _executeAsAsyncStream = typeof(Helper)
            .GetMethod(nameof(ExecuteAsAsyncStream), PrivateStatic);

        internal static Task ToListAsync(IQueryable source, CancellationToken cancellation)
            => (Task)_toListAsync
            .MakeGenericMethod(source.ElementType)
            .Invoke(null, new object[] { source, cancellation });

        /// <summary>
        /// Returns the generic <see cref="DbSet{T}"/> for the type specified.
        /// </summary>
        /// <returns>Returns an instance of type <see cref="DbSet{T}"/>.</returns>
        [SecuritySafeCritical]
        internal static Func<Type, IQueryable> GetQueryableSetProvider(this DbContext dbContext) => new QueryableSetProvider(dbContext).GetQueryableSet;

        [SecuritySafeCritical]
        private sealed class QueryableSetProvider
        {
            private readonly DbContext _dbContext;

            [SecuritySafeCritical]
            public QueryableSetProvider(DbContext dbContext)
            {
                _dbContext = dbContext;
            }

            [SecuritySafeCritical]
            public IQueryable GetQueryableSet(Type type)
            {
                var method = _dbSetGetter.MakeGenericMethod(type);
                var set = method.Invoke(_dbContext, null);
                return (IQueryable)set;
            }
        }

        public static IAsyncEnumerable<object?> ExecuteAsAsyncStream(this IQueryable queryable)
            => (IAsyncEnumerable<object?>)_executeAsAsyncStream
            .MakeGenericMethod(queryable.ElementType)
            .Invoke(null, new object[] { queryable });

        private static async IAsyncEnumerable<object?> ExecuteAsAsyncStream<T>(IQueryable<T> queryable)
        {
            await foreach (var item in queryable.AsAsyncEnumerable().ConfigureAwait(false))
            {
                yield return item;
            }
        }
    }
}
