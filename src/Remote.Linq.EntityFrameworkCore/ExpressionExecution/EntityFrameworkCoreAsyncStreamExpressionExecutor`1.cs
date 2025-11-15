// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.ExpressionExecution;

using Aqua.TypeExtensions;
using Microsoft.EntityFrameworkCore;
using Remote.Linq.EntityFrameworkCore.ExpressionVisitors;
using Remote.Linq.ExpressionExecution;
using System.Security;
using SystemLinq = System.Linq.Expressions;

public abstract class EntityFrameworkCoreAsyncStreamExpressionExecutor<TDataTranferObject> : AsyncStreamExpressionExecutor<IQueryable, TDataTranferObject>
{
    [SecuritySafeCritical]
    protected EntityFrameworkCoreAsyncStreamExpressionExecutor(DbContext dbContext, IExpressionFromRemoteLinqContext? context = null)
        : this(dbContext.GetQueryableSetProvider(), context)
    {
    }

    protected EntityFrameworkCoreAsyncStreamExpressionExecutor(Func<Type, IQueryable> queryableProvider, IExpressionFromRemoteLinqContext? context = null)
        : base(queryableProvider, context ?? new EntityFrameworkCoreExpressionTranslatorContext())
    {
    }

    protected override SystemLinq.Expression Prepare(SystemLinq.Expression expression)
        => base.Prepare(expression
            .WrapQueryableInClosure()
            .ReplaceIncludeQueryMethods());

    protected override IAsyncEnumerable<object?> ExecuteAsyncStream(SystemLinq.Expression expression, CancellationToken cancellation)
    {
        if (!expression.CheckNotNull().Type.Implements(typeof(IQueryable<>)))
        {
            throw new ArgumentException("Expression must be of type IQueryable<>", nameof(expression));
        }

        var queryable = (IQueryable)expression.CompileAndInvokeExpression()!;
        return queryable.ExecuteAsAsyncStreamWithCancellation(cancellation);
    }
}