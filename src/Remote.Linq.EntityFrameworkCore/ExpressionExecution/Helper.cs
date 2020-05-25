// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.ExpressionExecution
{
    using Aqua.TypeSystem;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Security;

    internal static class Helper
    {
        private static readonly System.Reflection.MethodInfo _dbSetGetter = typeof(DbContext)
            .GetTypeInfo()
            .GetDeclaredMethods(nameof(DbContext.Set))
            .Single(x => x.IsGenericMethod && x.GetGenericArguments().Length == 1 && x.GetParameters().Length == 0);

        private static readonly System.Reflection.MethodInfo _executeAsAsyncStream = typeof(Helper)
            .GetMethod(nameof(ExecuteAsAsyncStream), BindingFlags.NonPublic | BindingFlags.Static);

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
            await foreach (var item in queryable.AsAsyncEnumerable())
            {
                yield return item;
            }
        }
    }
}
