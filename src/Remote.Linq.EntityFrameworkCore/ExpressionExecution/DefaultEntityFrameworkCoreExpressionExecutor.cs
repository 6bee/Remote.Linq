// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.ExpressionExecution
{
    using Aqua.Dynamic;
    using Aqua.TypeExtensions;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Security;

    public class DefaultEntityFrameworkCoreExpressionExecutor : EntityFrameworkCoreExpressionExecutor<DynamicObject?>
    {
        private readonly IDynamicObjectMapper _mapper;
        private readonly Func<Type, bool> _setTypeInformation;

        [SecuritySafeCritical]
        public DefaultEntityFrameworkCoreExpressionExecutor(DbContext dbContext, IExpressionFromRemoteLinqContext? context = null, Func<Type, bool>? setTypeInformation = null)
            : this(dbContext.GetQueryableSetProvider(), context, setTypeInformation)
        {
        }

        public DefaultEntityFrameworkCoreExpressionExecutor(Func<Type, IQueryable> queryableProvider, IExpressionFromRemoteLinqContext? context = null, Func<Type, bool>? setTypeInformation = null)
            : this(0, queryableProvider, context ?? new EntityFrameworkCoreExpressionTranslatorContext(), setTypeInformation)
        {
        }

        private DefaultEntityFrameworkCoreExpressionExecutor(int n, Func<Type, IQueryable> queryableProvider, IExpressionFromRemoteLinqContext context, Func<Type, bool>? setTypeInformation)
            : base(queryableProvider, context)
        {
            _mapper = context.ValueMapper;
            _setTypeInformation = setTypeInformation ?? (t => !t.IsAnonymousType());
        }

        /// <summary>
        /// Converts the query result into a collection of <see cref="DynamicObject"/>.
        /// </summary>
        /// <param name="queryResult">The reult of the query execution.</param>
        /// <returns>The mapped query result.</returns>
        protected override DynamicObject? ConvertResult(object? queryResult)
            => _mapper.MapObject(queryResult, _setTypeInformation);
    }
}