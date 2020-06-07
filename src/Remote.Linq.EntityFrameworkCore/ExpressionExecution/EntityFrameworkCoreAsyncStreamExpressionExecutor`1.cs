// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.ExpressionExecution
{
    using Aqua.TypeSystem;
    using Aqua.TypeSystem.Extensions;
    using Microsoft.EntityFrameworkCore;
    using Remote.Linq.EntityFrameworkCore.ExpressionVisitors;
    using Remote.Linq.ExpressionExecution;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;

    public abstract class EntityFrameworkCoreAsyncStreamExpressionExecutor<TDataTranferObject> : AsyncStreamExpressionExecutor<TDataTranferObject>
        where TDataTranferObject : class
    {
        [SecuritySafeCritical]
        protected EntityFrameworkCoreAsyncStreamExpressionExecutor(DbContext dbContext, ITypeResolver? typeResolver = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            : this(dbContext.GetQueryableSetProvider(), typeResolver, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated))
        {
        }

        protected EntityFrameworkCoreAsyncStreamExpressionExecutor(Func<Type, IQueryable> queryableProvider, ITypeResolver? typeResolver = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            : base(queryableProvider, typeResolver, canBeEvaluatedLocally)
        {
        }

        protected override Expressions.Expression Prepare(Expressions.Expression expression)
            => base.Prepare(expression).ReplaceIncludeMethodCall();

        protected override IAsyncEnumerable<object?> ExecuteAsyncStream(System.Linq.Expressions.Expression expression)
        {
            if (!expression.Type.Implements(typeof(IQueryable<>)))
            {
                throw new ArgumentException("Expression must be of type IQueryable<>");
            }

            var queryable = (IQueryable)expression.CompileAndInvokeExpression() !;
            return queryable.ExecuteAsAsyncStream();
        }
    }
}
