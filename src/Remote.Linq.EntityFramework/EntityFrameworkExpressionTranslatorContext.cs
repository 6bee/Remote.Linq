// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using System;
    using SystemLinq = System.Linq.Expressions;

    public class EntityFrameworkExpressionTranslatorContext : ExpressionTranslatorContext
    {
        public EntityFrameworkExpressionTranslatorContext(ITypeInfoProvider? typeInfoProvider, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            : this(null, typeInfoProvider, null, canBeEvaluatedLocally)
        {
        }

        public EntityFrameworkExpressionTranslatorContext(ITypeResolver? typeResolver, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            : this(typeResolver, null, null, canBeEvaluatedLocally)
        {
        }

        public EntityFrameworkExpressionTranslatorContext(
            ITypeResolver? typeResolver = null,
            ITypeInfoProvider? typeInfoProvider = null,
            IIsKnownTypeProvider? isKnownTypeProvider = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
            IDynamicObjectMapper? valueMapper = null)
            : base(typeResolver, typeInfoProvider, isKnownTypeProvider, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated), valueMapper)
        {
        }
    }
}