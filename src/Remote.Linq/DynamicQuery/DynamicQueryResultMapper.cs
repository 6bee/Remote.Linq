// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using System;
    using SystemLinq = System.Linq.Expressions;

    public class DynamicQueryResultMapper : ExpressionTranslatorContext
    {
        public DynamicQueryResultMapper(
            ITypeResolver? typeResolver = null,
            ITypeInfoProvider? typeInfoProvider = null,
            IIsKnownTypeProvider? isKnownTypeProvider = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            : base(typeResolver, typeInfoProvider, isKnownTypeProvider, canBeEvaluatedLocally)
        {
        }
    }
}