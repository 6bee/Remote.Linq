// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework.ExpressionExecution
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Reflection;
    using System.Security;

    internal static class Helper
    {
        private static readonly MethodInfo _toListAsync = typeof(QueryableExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => string.Equals(m.Name, nameof(QueryableExtensions.ToListAsync), StringComparison.Ordinal))
            .Where(m => m.IsGenericMethodDefinition && m.GetParameters().Length == 2)
            .Single();

        private static readonly MethodInfo DbContextSetMethod = typeof(DbContext)
            .GetMethods()
            .Single(x => x.Name == nameof(DbContext.Set) && x.IsGenericMethod && x.GetGenericArguments().Length == 1 && x.GetParameters().Length == 0);

        internal static MethodInfo ToListAsync(Type elementType) => _toListAsync.MakeGenericMethod(elementType);

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
                var method = DbContextSetMethod.MakeGenericMethod(type);
                var set = method.Invoke(_dbContext, null);
                return (IQueryable)set;
            }
        }
    }
}
