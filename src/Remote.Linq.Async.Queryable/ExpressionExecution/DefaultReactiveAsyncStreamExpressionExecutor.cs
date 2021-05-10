// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Queryable.ExpressionExecution
{
    using Aqua.Dynamic;
    using Aqua.TypeExtensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class DefaultReactiveAsyncStreamExpressionExecutor : InteractiveAsyncStreamExpressionExecutor<DynamicObject>
    {
        private readonly IDynamicObjectMapper _mapper;
        private readonly Func<Type, bool> _setTypeInformation;

        public DefaultReactiveAsyncStreamExpressionExecutor(
            Func<Type, IAsyncQueryable> queryableProvider,
            IExpressionFromRemoteLinqContext? context = null,
            Func<Type, bool>? setTypeInformation = null)
            : this(0, queryableProvider, context ?? new DynamicAsyncQueryResultMapper(), setTypeInformation)
        {
        }

        private DefaultReactiveAsyncStreamExpressionExecutor(
            int n,
            Func<Type, IAsyncQueryable> queryableProvider,
            IExpressionFromRemoteLinqContext context,
            Func<Type, bool>? setTypeInformation = null)
            : base(queryableProvider, context)
        {
            _mapper = context.ValueMapper;
            _setTypeInformation = setTypeInformation ?? (t => !t.IsAnonymousType());
        }

        protected override async IAsyncEnumerable<DynamicObject> ConvertResult(IAsyncEnumerable<object?> queryResult)
        {
            if (queryResult is not null)
            {
                await foreach (var item in queryResult)
                {
                    yield return _mapper.MapObject(item, _setTypeInformation)
                        ?? DynamicObject.CreateDefault(Context.SystemExpression?.Type);
                }
            }
        }
    }
}