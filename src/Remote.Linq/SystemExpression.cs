// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq;

/// <summary>
/// Entry point for accessing factory and helper function for <see cref="System.Linq.Expressions.Expression"/>.
/// </summary>
public static class SystemExpression
{
    /// <summary>
    /// Gets a factory for creating <see cref="IQueryable{T}"/>
    /// (or <see cref="IQueryable"/> respectively) suited for remote execution.
    /// </summary>
    /// <remarks>
    /// Actual factory methods exist as extention methods.
    /// Make sure to add using for namespace containing targeted factory extension method (e.g. <i>using Remote.Linq;</i>).
    /// </remarks>
    public static SystemExpressionFactory Factory { get; } = new();
}