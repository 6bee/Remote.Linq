// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Minor Code Smell", "S4136:Method overloads should be grouped together", Justification = "Methods appear in logical order")]
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
