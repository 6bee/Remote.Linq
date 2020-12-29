// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// Provides factory methods for creating <see cref="IQueryable{T}"/>
    /// (or <see cref="IQueryable"/> respectively) suited for remote execution.
    /// The methods on this class are accessed via Remote.Linq.RemoteQueryable.Factory.
    /// </summary>
    public sealed class RemoteQueryableFactory
    {
        internal RemoteQueryableFactory()
        {
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string? ToString() => base.ToString();

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object? obj) => base.Equals(obj);

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => base.GetHashCode();
    }
}
