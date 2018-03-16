// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.Dynamic;
    using System.Collections.Generic;

    internal interface IExpressionExecutionDecorator
    {
        Expression Prepare(Expression expression);

        System.Linq.Expressions.Expression Transform(Expression expression);

        System.Linq.Expressions.Expression Prepare(System.Linq.Expressions.Expression expression);

        object Execute(System.Linq.Expressions.Expression expression);

        object ProcessResult(object queryResult);

        IEnumerable<DynamicObject> ConvertResult(object queryResult);

        IEnumerable<DynamicObject> ProcessResult(IEnumerable<DynamicObject> queryResult);
    }
}
