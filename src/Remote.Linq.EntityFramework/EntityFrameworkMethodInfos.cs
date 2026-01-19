// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework;

using Aqua.TypeExtensions;
using System.Linq.Expressions;
using System.Reflection;

internal static class EntityFrameworkMethodInfos
{
    /// <summary>
    /// Type definition used in generic type filters.
    /// </summary>
    private sealed class T;

    /// <summary>
    /// Type definition used in generic type filters.
    /// </summary>
    private sealed class TProperty;

    internal static readonly MethodInfo StringIncludeMethodInfo = typeof(System.Data.Entity.QueryableExtensions).GetMethodEx(
        nameof(System.Data.Entity.QueryableExtensions.Include),
        [typeof(T)],
        typeof(IQueryable<T>),
        typeof(string));

    internal static readonly MethodInfo IncludeMethodInfo = typeof(System.Data.Entity.QueryableExtensions).GetMethodEx(
        nameof(System.Data.Entity.QueryableExtensions.Include),
        [typeof(T), typeof(TProperty)],
        typeof(IQueryable<T>),
        typeof(Expression<Func<T, TProperty>>));
}