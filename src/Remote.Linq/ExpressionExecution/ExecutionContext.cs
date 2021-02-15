// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution
{
    using System.Collections.Generic;
    using RemoteLinq = Remote.Linq.Expressions;
    using SystemLinq = System.Linq.Expressions;

    /// <summary>
    /// <see cref="ExecutionContext"/> may be used to carry state between individual processing steps.
    /// </summary>
    public sealed class ExecutionContext
    {
        private Dictionary<string, object?>? _dictionary;

        private Dictionary<string, object?> Dictionary
            => _dictionary ?? (_dictionary = new Dictionary<string, object?>());

        /// <summary>
        /// Sets a key value pair.
        /// </summary>
        public void Set(string key, object? value)
            => Dictionary[key] = value;

        /// <summary>
        /// Tries to get a value previously set for the given key.
        /// </summary>
        public bool TryGet(string key, out object? value)
            => Dictionary.TryGetValue(key, out value);

        /// <summary>
        /// Gets the <see cref="RemoteLinq.Expression"/> of the current execution.
        /// </summary>
        public RemoteLinq.Expression? RemoteExpression { get; internal set; }

        /// <summary>
        /// Gets the <see cref="SystemLinq.Expression"/> of the current execution.
        /// </summary>
        public SystemLinq.Expression? SystemExpression { get; internal set; }
    }
}