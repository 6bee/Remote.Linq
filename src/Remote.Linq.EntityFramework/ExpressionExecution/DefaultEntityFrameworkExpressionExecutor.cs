// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework.ExpressionExecution
{
    using Aqua.Dynamic;
    using Aqua.TypeExtensions;
    using Remote.Linq.DynamicQuery;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Security;

    public class DefaultEntityFrameworkExpressionExecutor : EntityFrameworkExpressionExecutor<DynamicObject?>
    {
        private readonly IDynamicObjectMapper _mapper;
        private readonly Func<Type, bool> _setTypeInformation;

        [SecuritySafeCritical]
        public DefaultEntityFrameworkExpressionExecutor(
            DbContext dbContext,
            IExpressionTranslatorContext? context = null,
            Func<Type, bool>? setTypeInformation = null)
            : this(dbContext.GetQueryableSetProvider(), context, setTypeInformation)
        {
        }

        public DefaultEntityFrameworkExpressionExecutor(
            Func<Type, IQueryable> queryableProvider,
            IExpressionTranslatorContext? context = null,
            Func<Type, bool>? setTypeInformation = null)
            : base(queryableProvider, context)
        {
            _mapper = context?.ValueMapper ?? new DynamicQueryResultMapper();
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