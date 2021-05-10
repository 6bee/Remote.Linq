// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.ExpressionVisitors.ExpressionTranslator
{
    using System.Linq.Expressions;

    public abstract class ExpressionTranslatorTestBase
    {
        protected static (T Original, T RoundTrip) BackAndForth<T>(T expression, IExpressionTranslatorContext context = null)
            where T : Expression
        {
            var remoteExpression = expression.ToRemoteLinqExpression(context);
            var linqExpression = remoteExpression.ToLinqExpression(context);
            return (expression, (T)linqExpression);
        }
    }
}
