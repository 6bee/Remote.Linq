// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#if ASYNC_STREAM

namespace Remote.Linq.ExpressionExecution
{
    using System.Collections.Generic;

    internal interface IAsyncStreamExpressionExecutionDecorator<TDataTranferObject>
        where TDataTranferObject : class
    {
        Remote.Linq.Expressions.Expression Prepare(Remote.Linq.Expressions.Expression expression);

        System.Linq.Expressions.Expression Transform(Remote.Linq.Expressions.Expression expression);

        System.Linq.Expressions.Expression Prepare(System.Linq.Expressions.Expression expression);

        IAsyncEnumerable<object?> ExecuteAsyncStream(System.Linq.Expressions.Expression expression);

        IAsyncEnumerable<object?> ProcessResult(IAsyncEnumerable<object?> queryResult);

        IAsyncEnumerable<TDataTranferObject?> ConvertResult(IAsyncEnumerable<object?> queryResult);

        IAsyncEnumerable<TDataTranferObject?> ProcessResult(IAsyncEnumerable<TDataTranferObject?> queryResult);
    }
}

#endif // ASYNC_STREAM
