// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ExpressionExtensions
    {
        private static ExpressionExecutor CreateExecutor(Func<Type, IQueryable> queryableProvider, ITypeResolver? typeResolver, IDynamicObjectMapper? mapper, Func<Type, bool>? setTypeInformation, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally)
            => new ExpressionExecutor(queryableProvider, typeResolver, mapper, setTypeInformation, canBeEvaluatedLocally);

        public static ExpressionExecutionContext Executor(this Expression expression, Func<Type, IQueryable> queryableProvider, ITypeResolver? typeResolver = null, IDynamicObjectMapper? mapper = null, Func<Type, bool>? setTypeInformation = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => new ExpressionExecutionContext(CreateExecutor(queryableProvider, typeResolver, mapper, setTypeInformation, canBeEvaluatedLocally), expression);

        public static IEnumerable<DynamicObject?>? Execute(this Expression expression, Func<Type, IQueryable> queryableProvider, ITypeResolver? typeResolver = null, IDynamicObjectMapper? mapper = null, Func<Type, bool>? setTypeInformation = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateExecutor(queryableProvider, typeResolver, mapper, setTypeInformation, canBeEvaluatedLocally).Execute(expression);
    }
}
