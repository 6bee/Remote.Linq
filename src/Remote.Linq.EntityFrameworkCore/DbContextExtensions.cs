// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore;

using Aqua.TypeExtensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Reflection;
using System.Security;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class DbContextExtensions
{
    /// <summary>
    /// Type definition used in generic type filters.
    /// </summary>
    private sealed class TEntity;

    private static readonly MethodInfo _dbContextSetMethod = typeof(DbContext).GetMethodEx(
        nameof(DbContext.Set),
        genericArguments: [typeof(TEntity)]);

    /// <summary>
    /// Returns the generic <see cref="DbSet{T}"/> for the type specified.
    /// </summary>
    /// <returns>Returns an instance of type <see cref="DbSet{T}"/>.</returns>
    [SecuritySafeCritical]
    public static Func<Type, IQueryable> GetQueryableSetProvider(this DbContext dbContext)
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
            var method = _dbContextSetMethod.MakeGenericMethod(type);
            var set = method.Invoke(_dbContext, null);
            return (IQueryable)set!;
        }
    }
}