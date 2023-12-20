// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution;

using System.Diagnostics.CodeAnalysis;
using RemoteLinq = Remote.Linq.Expressions;
using SystemLinq = System.Linq.Expressions;

[SuppressMessage("Minor Code Smell", "S4136:Method overloads should be grouped together", Justification = "Methods appear in logical order")]
internal interface IExpressionExecutionDecorator<TDataTranferObject>
{
    ExecutionContext ExecutionContext { get; }

    RemoteLinq.Expression Prepare(RemoteLinq.Expression expression);

    SystemLinq.Expression Transform(RemoteLinq.Expression expression);

    SystemLinq.Expression Prepare(SystemLinq.Expression expression);

    object? Execute(SystemLinq.Expression expression);

    object? ProcessResult(object? queryResult);

    TDataTranferObject ConvertResult(object? queryResult);

    TDataTranferObject ProcessResult(TDataTranferObject queryResult);
}