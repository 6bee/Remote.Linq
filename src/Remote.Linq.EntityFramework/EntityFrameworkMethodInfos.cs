// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework;

using Aqua.TypeExtensions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

internal static class EntityFrameworkMethodInfos
{
    /// <summary>
    /// Type definition used in generic type filters.
    /// </summary>
    [SuppressMessage("Major Bug", "S3453:Classes should not have only \"private\" constructors", Justification = "For reflection only")]
    private sealed class T
    {
        private T()
        {
        }
    }

    /// <summary>
    /// Type definition used in generic type filters.
    /// </summary>
    [SuppressMessage("Major Bug", "S3453:Classes should not have only \"private\" constructors", Justification = "For reflection only")]
    private sealed class TProperty
    {
        private TProperty()
        {
        }
    }

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