// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.TypeSystem;
    using System;

    /// <summary>
    /// Denotes a context for translating <i>System.Linq.Expressions</i> to <i>Remote.Linq</i>.
    /// </summary>
    public interface IExpressionToRemoteLinqContext : IExpressionValueMapperProvider
    {
        /// <summary>
        /// Gets a provider for translating <see cref="Type"/> to <see cref="TypeInfo"/>.
        /// </summary>
        ITypeInfoProvider TypeInfoProvider { get; }

        /// <summary>
        /// Gets a function to check whether a value requires mapping.
        /// </summary>
        Func<object, bool> NeedsMapping { get; }
    }
}