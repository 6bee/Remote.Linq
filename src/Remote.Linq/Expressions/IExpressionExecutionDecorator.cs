// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.Dynamic;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    internal interface IExpressionExecutionDecorator
    {
        Expression Prepare(Expression expression);

        System.Linq.Expressions.Expression Transform(Expression expression);

        System.Linq.Expressions.Expression Prepare(System.Linq.Expressions.Expression expression);

        object? Execute(System.Linq.Expressions.Expression expression);

        [return: NotNullIfNotNull("queryResult")]
        object? ProcessResult(object? queryResult);

        [return: NotNullIfNotNull("queryResult")]
        IEnumerable<DynamicObject?>? ConvertResult(object? queryResult);

        [return: NotNullIfNotNull("queryResult")]
        IEnumerable<DynamicObject?>? ProcessResult(IEnumerable<DynamicObject?>? queryResult);
    }
}
