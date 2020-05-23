// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using System.Threading;
    using System.Threading.Tasks;

    internal interface IAsyncExpressionExecutionDecorator<TDataTranferObject> : IExpressionExecutionDecorator<TDataTranferObject>
    {
        System.Linq.Expressions.Expression PrepareAsyncQuery(System.Linq.Expressions.Expression expression, CancellationToken cancellationToken);

        Task<object?> ExecuteAsync(System.Linq.Expressions.Expression expression, CancellationToken cancellationToken);
    }
}
