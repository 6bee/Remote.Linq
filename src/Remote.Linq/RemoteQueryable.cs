// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq;

using Aqua.TypeSystem;
using SystemLinq = System.Linq.Expressions;

/// <summary>
/// Entry point for creating <see cref="IQueryable{T}"/> instance for remote execution.
/// </summary>
public static class RemoteQueryable
{
    /// <summary>
    /// Gets a factory for creating <see cref="IQueryable{T}"/>
    /// (or <see cref="IQueryable"/> respectively) suited for remote execution.
    /// </summary>
    /// <remarks>
    /// Actual factory methods exist as extention methods.
    /// Make sure to add using for namespace containing targeted factory extension method (e.g. <i>using Remote.Linq;</i>).
    /// </remarks>
    public static RemoteQueryableFactory Factory { get; } = new();

    internal static ExpressionTranslatorContext? GetExpressionTranslatorContextOrNull(ITypeInfoProvider? typeInfoProvider, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally)
        => typeInfoProvider is null && canBeEvaluatedLocally is null ? null : new ExpressionTranslatorContext(typeInfoProvider, canBeEvaluatedLocally);
}