// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.ExpressionExecution
{
    using Aqua.Dynamic;
    using Aqua.TypeExtensions;
    using Aqua.TypeSystem;
    using Microsoft.EntityFrameworkCore;
    using Remote.Linq.DynamicQuery;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;

    public class DefaultEntityFrameworkCoreAsyncStreamExpressionExecutor : EntityFrameworkCoreAsyncStreamExpressionExecutor<DynamicObject>
    {
        private readonly IDynamicObjectMapper _mapper;
        private readonly Func<Type, bool> _setTypeInformation;

        [SecuritySafeCritical]
        public DefaultEntityFrameworkCoreAsyncStreamExpressionExecutor(DbContext dbContext, ITypeResolver? typeResolver = null, IDynamicObjectMapper? mapper = null, Func<Type, bool>? setTypeInformation = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            : this(dbContext.GetQueryableSetProvider(), typeResolver, mapper, setTypeInformation, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated))
        {
        }

        public DefaultEntityFrameworkCoreAsyncStreamExpressionExecutor(Func<Type, IQueryable> queryableProvider, ITypeResolver? typeResolver = null, IDynamicObjectMapper? mapper = null, Func<Type, bool>? setTypeInformation = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
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
