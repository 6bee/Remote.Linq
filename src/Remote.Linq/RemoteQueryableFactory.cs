// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// Provides factory methods for creating <see cref="IQueryable{T}"/> types
    /// (or <see cref="IQueryable"/> respectively) suited for remote execution.
    /// Extension methods on this type are accessed via <see cref="RemoteQueryable.Factory"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class RemoteQueryableFactory
    {
        internal RemoteQueryableFactory()
        {
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => $"{typeof(RemoteQueryable)}.{nameof(RemoteQueryable.Factory)}";

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object? obj) => obj is RemoteQueryableFactory;

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => 0;
    }
}