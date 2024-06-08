// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution;

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

    ExecutionContext IExpressionExecutionDecorator<TDataTranferObject>.ExecutionContext => _parent.ExecutionContext;

    internal ExecutionContext Context => _parent.ExecutionContext;

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

        var processedDataTranferObjects = ProcessConvertedResult(dataTranferObjects);
        return processedDataTranferObjects;
    }

    /// <summary>
    /// Prepare remote linq expression before transformation to system linq expression.
    /// </summary>
    /// <remarks>
    /// Used for synchronous and asynchronous execution.
    /// </remarks>
    protected virtual RemoteLinq.Expression Prepare(RemoteLinq.Expression expression)
        => _parent.Prepare(expression);

    /// <summary>
    /// Transform remote linq expression to system linq expression.
    /// </summary>
    /// <remarks>
    /// Used for synchronous and asynchronous execution.
    /// </remarks>
    protected virtual SystemLinq.Expression Transform(RemoteLinq.Expression expression)
        => _parent.Transform(expression);

    /// <summary>
    /// Prepare system linq rexpression befor execution.
    /// </summary>
    /// <remarks>
    /// Used for synchronous and asynchronous execution.
    /// </remarks>
    protected virtual SystemLinq.Expression Prepare(SystemLinq.Expression expression)
        => _parent.Prepare(expression);

    /// <summary>
    /// Execute system linq expression and retrieve result.
    /// </summary>
    /// <remarks>
    /// Used for synchronous execution only.
    /// </remarks>
    protected virtual object? Execute(SystemLinq.Expression expression)
        => _parent.Execute(expression);

    /// <summary>
    /// Process result as retrieved from expression execution.
    /// </summary>
    /// <remarks>
    /// Used for synchronous and asynchronous execution.
    /// </remarks>
    protected virtual object? ProcessResult(object? queryResult)
        => _parent.ProcessResult(queryResult);

    /// <summary>
    /// Convert result to target type.
    /// </summary>
    /// <remarks>
    /// Used for synchronous and asynchronous execution.
    /// </remarks>
    protected virtual TDataTranferObject ConvertResult(object? queryResult)
        => _parent.ConvertResult(queryResult);

    /// <summary>
    /// Prepare result before being returned.
    /// </summary>
    /// <remarks>
    /// Used for synchronous and asynchronous execution.
    /// </remarks>
    protected virtual TDataTranferObject ProcessConvertedResult(TDataTranferObject queryResult)
        => _parent.ProcessConvertedResult(queryResult);

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

    TDataTranferObject IExpressionExecutionDecorator<TDataTranferObject>.ProcessConvertedResult(TDataTranferObject queryResult)
        => ProcessConvertedResult(queryResult);
}