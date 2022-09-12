// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.TypeExtensions;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Security;

    [SecuritySafeCritical]
    public sealed class QueryableSetProvider
    {
        private class TEntity
        {
        }

        private static readonly MethodInfo _dbSetMethod = typeof(QueryableSetProvider).GetMethodEx(
            nameof(DbContext.Set),
            genericArguments: new[] { typeof(TEntity) });

        private readonly DbContext _dbContext;

        [SecuritySafeCritical]
        public QueryableSetProvider(DbContext dbContext)
            => _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        [SecuritySafeCritical]
        public IQueryable GetQueryableSet(Type type)
        {
            var method = _dbSetMethod.MakeGenericMethod(type);
            var set = method.Invoke(this, null);
            return (IQueryable)set!;
        }

        [SecuritySafeCritical]
        private DbSet<T> Set<T>()
            where T : class
            => _dbContext.Set<T>();
    }
}