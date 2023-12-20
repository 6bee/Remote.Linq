// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution;

using System.Threading;
using System.Threading.Tasks;
using SystemLinq = System.Linq.Expressions;

internal interface IAsyncExpressionExecutionDecorator<TDataTranferObject> : IExpressionExecutionDecorator<TDataTranferObject>
{
    SystemLinq.Expression PrepareAsyncQuery(SystemLinq.Expression expression, CancellationToken cancellation);

    ValueTask<object?> ExecuteAsync(SystemLinq.Expression expression, CancellationToken cancellation);
}