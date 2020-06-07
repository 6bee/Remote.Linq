// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution
{
    using Remote.Linq.Expressions;
    using System;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Minor Code Smell", "S4136:Method overloads should be grouped together", Justification = "Methods appear in logical order")]
    public abstract class ExpressionExecutionDecoratorBase<TDataTranferObject> : IExpressionExecutionDecorator<TDataTranferObject>
    {
        private readonly IExpressionExecutionDecorator<TDataTranferObject> _parent;

        protected ExpressionExecutionDecoratorBase(ExpressionExecutionDecorator<TDataTranferObject> parent)
            : this((IExpressionExecutionDecorator<TDataTranferObject>)parent)
        {
        }

        protected ExpressionExecutionDecoratorBase(DefaultExpressionExecutor parent)
            : this((IExpressionExecutionDecorator<TDataTranferObject>)parent)
        {
        }

        [SuppressMessage("Major Code Smell", "S3442:\"abstract\" classes should not have \"public\" constructors", Justification = "Argument type has internal visibility only")]
        internal ExpressionExecutionDecoratorBase(IExpressionExecutionDecorator<TDataTranferObject> parent)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        protected TDataTranferObject Execute(Expression expression)
        {
            var preparedRemoteExpression = Prepare(expression);
            var linqExpression = Transform(preparedRemoteExpression);
            var preparedLinqExpression = Prepare(linqExpression);
            var queryResult = Execute(preparedLinqExpression);
            var processedResult = ProcessResult(queryResult);
            var dataTranferObjects = ConvertResult(processedResult);
            var processedDataTranferObjects = ProcessResult(dataTranferObjects);
            return processedDataTranferObjects;
        }

        protected virtual Expression Prepare(Expression expression) => _parent.Prepare(expression);

        protected virtual System.Linq.Expressions.Expression Transform(Expression expression) => _parent.Transform(expression);

        protected virtual System.Linq.Expressions.Expression Prepare(System.Linq.Expressions.Expression expression) => _parent.Prepare(expression);

        protected virtual object? Execute(System.Linq.Expressions.Expression expression) => _parent.Execute(expression);

        protected virtual object? ProcessResult(object? queryResult) => _parent.ProcessResult(queryResult);

        protected virtual TDataTranferObject ConvertResult(object? queryResult) => _parent.ConvertResult(queryResult);

        protected virtual TDataTranferObject ProcessResult(TDataTranferObject queryResult) => _parent.ProcessResult(queryResult);

        Expression IExpressionExecutionDecorator<TDataTranferObject>.Prepare(Expression expression) => Prepare(expression);

        System.Linq.Expressions.Expression IExpressionExecutionDecorator<TDataTranferObject>.Transform(Expression expression) => Transform(expression);

        System.Linq.Expressions.Expression IExpressionExecutionDecorator<TDataTranferObject>.Prepare(System.Linq.Expressions.Expression expression) => Prepare(expression);

        object? IExpressionExecutionDecorator<TDataTranferObject>.Execute(System.Linq.Expressions.Expression expression) => Execute(expression);

        object? IExpressionExecutionDecorator<TDataTranferObject>.ProcessResult(object? queryResult) => ProcessResult(queryResult);

        TDataTranferObject IExpressionExecutionDecorator<TDataTranferObject>.ConvertResult(object? queryResult) => ConvertResult(queryResult);

        TDataTranferObject IExpressionExecutionDecorator<TDataTranferObject>.ProcessResult(TDataTranferObject queryResult) => ProcessResult(queryResult);
    }
}
