// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework;

using Aqua.TypeExtensions;
using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Security;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class DbContextExtensions
{
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