// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.TypeSystem;
    using System;

    /// <summary>
    /// Denotes a context for translating <i>Remote.Linq.Expressions</i> to <i>System.Linq</i>.
    /// </summary>
    public interface IExpressionFromRemoteLinqContext : IExpressionValueMapperProvider
    {
        /// <summary>
        /// Gets a type resolver to resolve <see cref="TypeInfo"/> to <see cref="Type"/>.
        /// </summary>
        ITypeResolver TypeResolver { get; }
    }
}