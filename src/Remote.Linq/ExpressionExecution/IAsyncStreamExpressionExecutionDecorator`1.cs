// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution;

using System.Diagnostics.CodeAnalysis;
using RemoteLinq = Remote.Linq.Expressions;
using SystemLinq = System.Linq.Expressions;

[SuppressMessage("Minor Code Smell", "S4136:Method overloads should be grouped together", Justification = "Methods appear in logical order")]
internal interface IAsyncStreamExpressionExecutionDecorator<TDataTranferObject>
{
    ExecutionContext Context { get; }

    RemoteLinq.Expression Prepare(RemoteLinq.Expression expression);

    SystemLinq.Expression Transform(RemoteLinq.Expression expression);

    SystemLinq.Expression Prepare(SystemLinq.Expression expression);

    IAsyncEnumerable<object?> ExecuteAsyncStream(SystemLinq.Expression expression, CancellationToken cancellation);

    IAsyncEnumerable<object?> ProcessResult(IAsyncEnumerable<object?> queryResult);

    IAsyncEnumerable<TDataTranferObject> ConvertResult(IAsyncEnumerable<object?> queryResult);

    IAsyncEnumerable<TDataTranferObject> ProcessConvertedResult(IAsyncEnumerable<TDataTranferObject> queryResult);
}