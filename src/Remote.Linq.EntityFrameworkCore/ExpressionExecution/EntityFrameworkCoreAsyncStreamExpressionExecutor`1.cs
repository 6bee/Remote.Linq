// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.ExpressionExecution
{
    using Aqua.TypeExtensions;
    using Aqua.TypeSystem;
    using Microsoft.EntityFrameworkCore;
    using Remote.Linq.EntityFrameworkCore.ExpressionVisitors;
    using Remote.Linq.ExpressionExecution;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading;
    using RemoteLinq = Remote.Linq.Expressions;
    using SystemLinq = System.Linq.Expressions;

    public abstract class EntityFrameworkCoreAsyncStreamExpressionExecutor<TDataTranferObject> : AsyncStreamExpressionExecutor<IQueryable, TDataTranferObject>
        where TDataTranferObject : class
    {
        [SecuritySafeCritical]
        protected EntityFrameworkCoreAsyncStreamExpressionExecutor(DbContext dbContext, ITypeResolver? typeResolver = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            : this(dbContext.GetQueryableSetProvider(), typeResolver, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated))
        {
        }

        protected EntityFrameworkCoreAsyncStreamExpressionExecutor(Func<Type, IQueryable> queryableProvider, ITypeResolver? typeResolver = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            : base(queryableProvider, typeResolver, canBeEvaluatedLocally)
        {
        }

        protected override RemoteLinq.Expression Prepare(RemoteLinq.Expression expression)
            => base.Prepare(expression).ReplaceIncludeMethodCall();

        protected override IAsyncEnumerable<object?> ExecuteAsyncStream(SystemLinq.Expression expression, CancellationToken cancellation)
        {
            if (!expression.CheckNotNull(nameof(expression)).Type.Implements(typeof(IQueryable<>)))
            {
                throw new ArgumentException("Expression must be of type IQueryable<>", nameof(expression));
            }

            var queryable = (IQueryable)expression.CompileAndInvokeExpression() !;
            return queryable.ExecuteAsAsyncStreamWithCancellation(cancellation);
        }
    }
}
