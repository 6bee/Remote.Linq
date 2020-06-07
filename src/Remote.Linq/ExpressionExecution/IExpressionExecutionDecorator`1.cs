// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Minor Code Smell", "S4136:Method overloads should be grouped together", Justification = "Methods appear in logical order")]
    internal interface IExpressionExecutionDecorator<TDataTranferObject>
    {
        Remote.Linq.Expressions.Expression Prepare(Remote.Linq.Expressions.Expression expression);

        System.Linq.Expressions.Expression Transform(Remote.Linq.Expressions.Expression expression);

        System.Linq.Expressions.Expression Prepare(System.Linq.Expressions.Expression expression);

        object? Execute(System.Linq.Expressions.Expression expression);

        object? ProcessResult(object? queryResult);

        TDataTranferObject ConvertResult(object? queryResult);

        TDataTranferObject ProcessResult(TDataTranferObject queryResult);
    }
}
