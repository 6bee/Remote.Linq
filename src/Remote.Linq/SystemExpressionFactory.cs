// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq;

using System.ComponentModel;

/// <summary>
/// Provides factory and helper methods for <see cref="System.Linq.Expressions.Expression"/>.
/// Extension methods on this type are accessed via <see cref="SystemExpression.Factory"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class SystemExpressionFactory
{
    internal SystemExpressionFactory()
    {
    }

    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string ToString() => $"{typeof(SystemExpression)}.{nameof(SystemExpression.Factory)}";

    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object? obj) => obj is SystemExpressionFactory;

    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => 0;
}