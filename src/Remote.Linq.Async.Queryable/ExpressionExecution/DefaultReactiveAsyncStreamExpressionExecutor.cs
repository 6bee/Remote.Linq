// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Queryable.ExpressionExecution
{
    using Aqua.Dynamic;
    using Aqua.TypeExtensions;
    using Aqua.TypeSystem;
    using Remote.Linq.DynamicQuery;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    internal sealed class DefaultReactiveAsyncStreamExpressionExecutor : ReactiveAsyncStreamExpressionExecutor<DynamicObject>
    {
        private readonly IDynamicObjectMapper _mapper;
        private readonly Func<Type, bool> _setTypeInformation;

        public DefaultReactiveAsyncStreamExpressionExecutor(Func<Type, IAsyncQueryable> queryableProvider, ITypeResolver? typeResolver = null, IDynamicObjectMapper? mapper = null, Func<Type, bool>? setTypeInformation = null, Func<Expression, bool>? canBeEvaluatedLocally = null)
            : base(queryableProvider, typeResolver, canBeEvaluatedLocally)
        {
            _mapper = mapper ?? new DynamicQueryResultMapper();
            _setTypeInformation = setTypeInformation ?? (t => !t.IsAnonymousType());
        }

        protected override async IAsyncEnumerable<DynamicObject?> ConvertResult(IAsyncEnumerable<object?> queryResult)
        {
            await foreach (var item in queryResult.CheckNotNull(nameof(queryResult)))
            {
                yield return _mapper.MapObject(item, _setTypeInformation);
            }
        }
    }
}
