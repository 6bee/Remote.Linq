// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.TypeSystem;
    using System;
    using System.ComponentModel;
    using System.Linq;
    using SystemLinq = System.Linq.Expressions;

    public static class RemoteQueryable
    {
        /// <summary>
        /// Gets a factory for creating <see cref="IQueryable{T}"/>
        /// (or <see cref="IQueryable"/> respectively) suited for remote execution.
        /// </summary>
        /// <remarks>
        /// Make sure to add using for namespace containing targeted factory extension method (e.g. <i>using Remote.Linq;</i>).
        /// </remarks>
        public static RemoteQueryableFactory Factory { get; } = new RemoteQueryableFactory();

        internal static ExpressionTranslatorContext? GetExpressionTranslatorContextOrNull(ITypeInfoProvider? typeInfoProvider, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally)
            => typeInfoProvider is null && canBeEvaluatedLocally is null ? null : new ExpressionTranslatorContext(typeInfoProvider, canBeEvaluatedLocally);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static new bool Equals(object? objA, object? objB)
            => object.Equals(objA, objB);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static new bool ReferenceEquals(object? objA, object? objB)
            => object.ReferenceEquals(objA, objB);
    }
}