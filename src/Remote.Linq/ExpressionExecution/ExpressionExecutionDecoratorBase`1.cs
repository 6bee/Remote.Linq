// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution
{
    using System.Diagnostics.CodeAnalysis;
    using RemoteLinq = Remote.Linq.Expressions;
    using SystemLinq = System.Linq.Expressions;

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
            => _parent = parent.CheckNotNull();

        ExecutionContext IExpressionExecutionDecorator<TDataTranferObject>.Context => _parent.Context;

        internal ExecutionContext Context => _parent.Context;

        protected TDataTranferObject Execute(RemoteLinq.Expression expression)
        {
            var ctx = Context;

            var preparedRemoteExpression = Prepare(expression);
            ctx.RemoteExpression = preparedRemoteExpression;

            var linqExpression = Transform(preparedRemoteExpression);

            var preparedLinqExpression = Prepare(linqExpression);
            ctx.SystemExpression = preparedLinqExpression;

            var queryResult = Execute(preparedLinqExpression);

            var processedResult = ProcessResult(queryResult);

            var dataTranferObjects = ConvertResult(processedResult);

            var processedDataTranferObjects = ProcessResult(dataTranferObjects);
            return processedDataTranferObjects;
        }

        protected virtual RemoteLinq.Expression Prepare(RemoteLinq.Expression expression)
            => _parent.Prepare(expression);

        protected virtual SystemLinq.Expression Transform(RemoteLinq.Expression expression)
            => _parent.Transform(expression);

        protected virtual SystemLinq.Expression Prepare(SystemLinq.Expression expression)
            => _parent.Prepare(expression);

        protected virtual object? Execute(SystemLinq.Expression expression)
            => _parent.Execute(expression);

        protected virtual object? ProcessResult(object? queryResult)
            => _parent.ProcessResult(queryResult);

        protected virtual TDataTranferObject ConvertResult(object? queryResult)
            => _parent.ConvertResult(queryResult);

        protected virtual TDataTranferObject ProcessResult(TDataTranferObject queryResult)
            => _parent.ProcessResult(queryResult);

        RemoteLinq.Expression IExpressionExecutionDecorator<TDataTranferObject>.Prepare(RemoteLinq.Expression expression)
            => Prepare(expression);

        SystemLinq.Expression IExpressionExecutionDecorator<TDataTranferObject>.Transform(RemoteLinq.Expression expression)
            => Transform(expression);

        SystemLinq.Expression IExpressionExecutionDecorator<TDataTranferObject>.Prepare(SystemLinq.Expression expression)
            => Prepare(expression);

        object? IExpressionExecutionDecorator<TDataTranferObject>.Execute(SystemLinq.Expression expression)
            => Execute(expression);

        object? IExpressionExecutionDecorator<TDataTranferObject>.ProcessResult(object? queryResult)
            => ProcessResult(queryResult);

        TDataTranferObject IExpressionExecutionDecorator<TDataTranferObject>.ConvertResult(object? queryResult)
            => ConvertResult(queryResult);

        TDataTranferObject IExpressionExecutionDecorator<TDataTranferObject>.ProcessResult(TDataTranferObject queryResult)
            => ProcessResult(queryResult);
    }
}
