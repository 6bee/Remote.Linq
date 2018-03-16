// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.Dynamic;
    using System;
    using System.Collections.Generic;

    public abstract class ExpressionExecutionDecoratorBase : IExpressionExecutionDecorator
    {
        private readonly IExpressionExecutionDecorator _parent;

        protected ExpressionExecutionDecoratorBase(ExpressionExecutionDecorator parent)
            : this((IExpressionExecutionDecorator)parent)
        {
        }

        protected ExpressionExecutionDecoratorBase(ExpressionExecutor parent)
            : this((IExpressionExecutionDecorator)parent)
        {
        }

        internal ExpressionExecutionDecoratorBase(IExpressionExecutionDecorator parent)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        protected IEnumerable<DynamicObject> Execute(Expression expression)
        {
            var preparedRemoteExpression = Prepare(expression);
            var linqExpression = Transform(preparedRemoteExpression);
            var preparedLinqExpression = Prepare(linqExpression);
            var queryResult = Execute(preparedLinqExpression);
            var processedResult = ProcessResult(queryResult);
            var dynamicObjects = ConvertResult(processedResult);
            var processedDynamicObjects = ProcessResult(dynamicObjects);
            return processedDynamicObjects;
        }

        protected virtual Expression Prepare(Expression expression)
            => _parent.Prepare(expression);

        protected virtual System.Linq.Expressions.Expression Transform(Expression expression)
            => _parent.Transform(expression);

        protected virtual System.Linq.Expressions.Expression Prepare(System.Linq.Expressions.Expression expression)
            => _parent.Prepare(expression);

        protected virtual object Execute(System.Linq.Expressions.Expression expression)
            => _parent.Execute(expression);

        protected virtual object ProcessResult(object queryResult)
            => _parent.ProcessResult(queryResult);

        protected virtual IEnumerable<DynamicObject> ConvertResult(object queryResult)
            => _parent.ConvertResult(queryResult);

        protected virtual IEnumerable<DynamicObject> ProcessResult(IEnumerable<DynamicObject> queryResult)
            => _parent.ProcessResult(queryResult);

        Expression IExpressionExecutionDecorator.Prepare(Expression expression)
            => Prepare(expression);

        System.Linq.Expressions.Expression IExpressionExecutionDecorator.Transform(Expression expression)
            => Transform(expression);

        System.Linq.Expressions.Expression IExpressionExecutionDecorator.Prepare(System.Linq.Expressions.Expression expression)
            => Prepare(expression);

        object IExpressionExecutionDecorator.Execute(System.Linq.Expressions.Expression expression)
            => Execute(expression);

        object IExpressionExecutionDecorator.ProcessResult(object queryResult)
            => ProcessResult(queryResult);

        IEnumerable<DynamicObject> IExpressionExecutionDecorator.ConvertResult(object queryResult)
            => ConvertResult(queryResult);

        IEnumerable<DynamicObject> IExpressionExecutionDecorator.ProcessResult(IEnumerable<DynamicObject> queryResult)
            => ProcessResult(queryResult);
    }
}
