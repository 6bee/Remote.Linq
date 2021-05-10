// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.TypeSystem;
    using System;
    using System.Linq;
    using SystemLinq = System.Linq.Expressions;

    public static class RemoteQueryable
    {
        /// <summary>
        /// Gets a factory for creating <see cref="IQueryable{T}"/>
        /// (or <see cref="IQueryable"/> respectively) suited for remote execution.
        /// </summary>
        public static RemoteQueryableFactory Factory { get; } = new RemoteQueryableFactory();

        internal static ExpressionTranslatorContext? GetExpressionTRanslatorContextOrNull(ITypeInfoProvider? typeInfoProvider, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally)
            => typeInfoProvider is null && canBeEvaluatedLocally is null ? null : new ExpressionTranslatorContext(typeInfoProvider, canBeEvaluatedLocally);
    }
}
