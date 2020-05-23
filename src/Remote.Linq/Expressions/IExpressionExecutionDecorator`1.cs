// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    internal interface IExpressionExecutionDecorator<TDataTranferObject>
    {
        Expression Prepare(Expression expression);

        System.Linq.Expressions.Expression Transform(Expression expression);

        System.Linq.Expressions.Expression Prepare(System.Linq.Expressions.Expression expression);

        object? Execute(System.Linq.Expressions.Expression expression);

        object? ProcessResult(object? queryResult);

        TDataTranferObject ConvertResult(object? queryResult);

        TDataTranferObject ProcessResult(TDataTranferObject queryResult);
    }
}
