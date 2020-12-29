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

    public class DefaultReactiveAsyncExpressionExecutor : ReactiveAsyncExpressionExecutor<IEnumerable<DynamicObject?>?>
    {
        private readonly IDynamicObjectMapper _mapper;
        private readonly Func<Type, bool> _setTypeInformation;

        public DefaultReactiveAsyncExpressionExecutor(Func<Type, IAsyncQueryable> queryableProvider, ITypeResolver? typeResolver = null, IDynamicObjectMapper? mapper = null, Func<Type, bool>? setTypeInformation = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            : base(queryableProvider, typeResolver, canBeEvaluatedLocally)
        {
            _mapper = mapper ?? new DynamicQueryResultMapper();
            _setTypeInformation = setTypeInformation ?? (t => !t.IsAnonymousType());
        }

        /// <summary>
        /// Converts the query result into a collection of <see cref="DynamicObject"/>.
        /// </summary>
        /// <param name="queryResult">The reult of the query execution.</param>
        /// <returns>The mapped query result.</returns>
        protected override IEnumerable<DynamicObject?>? ConvertResult(object? queryResult)
            => _mapper.MapCollection(queryResult, _setTypeInformation);
    }
}
