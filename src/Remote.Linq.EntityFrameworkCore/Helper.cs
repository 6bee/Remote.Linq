// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore
{
    using Aqua.TypeSystem;
    using System;
    using SystemLinq = System.Linq.Expressions;

    internal static class Helper
    {
        internal static IExpressionToRemoteLinqContext? GetExpressionToRemoteLinqContext(ITypeInfoProvider? typeInfoProvider, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally)
            => typeInfoProvider is null && canBeEvaluatedLocally is null
            ? null
            : new EntityFrameworkCoreExpressionTranslatorContext(typeInfoProvider, canBeEvaluatedLocally);

        internal static IExpressionToRemoteLinqContext GetOrCreateContext(IExpressionToRemoteLinqContext? context)
            => context ?? new EntityFrameworkCoreExpressionTranslatorContext();
    }
}