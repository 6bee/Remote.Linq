// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution;

using System.ComponentModel;
using RemoteLinq = Remote.Linq.Expressions;
using SystemLinq = System.Linq.Expressions;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class ExpressionExecutionContextExtensions
{
    /// <summary>
    /// Decorate with custom strategy: prepare remote expression.
    /// </summary>
    public static ExpressionExecutionContext<TDataTranferObject> With<TDataTranferObject>(
        this ExpressionExecutionContext<TDataTranferObject> context,
        Func<RemoteLinq.Expression, RemoteLinq.Expression> transform)
        => new(((ExpressionExecutionDecoratorBase<TDataTranferObject>)context).With(transform), context.Expression);

    /// <summary>
    /// Replace expression transformtion logic with custom strategy: transform remote expression to system expression.
    /// </summary>
    public static ExpressionExecutionContext<TDataTranferObject> With<TDataTranferObject>(
        this ExpressionExecutionContext<TDataTranferObject> context,
        Func<RemoteLinq.Expression, SystemLinq.Expression> transform)
        => new(((ExpressionExecutionDecoratorBase<TDataTranferObject>)context).With(transform), context.Expression);

    /// <summary>
    /// Decorate with custom strategy: prepare system expression.
    /// </summary>
    public static ExpressionExecutionContext<TDataTranferObject> With<TDataTranferObject>(
        this ExpressionExecutionContext<TDataTranferObject> context,
        Func<SystemLinq.Expression, SystemLinq.Expression> transform)
        => new(((ExpressionExecutionDecoratorBase<TDataTranferObject>)context).With(transform), context.Expression);

    /// <summary>
    /// Replace expression execution logic with custom strategy: execute expression.
    /// </summary>
    public static ExpressionExecutionContext<TDataTranferObject> With<TDataTranferObject>(
        this ExpressionExecutionContext<TDataTranferObject> context,
        Func<SystemLinq.Expression, object?> transform)
        => new(((ExpressionExecutionDecoratorBase<TDataTranferObject>)context).With(transform), context.Expression);

    /// <summary>
    /// Decorate with custom strategy: process execution result.
    /// </summary>
    public static ExpressionExecutionContext<TDataTranferObject> With<TDataTranferObject>(
        this ExpressionExecutionContext<TDataTranferObject> context,
        Func<object?, object?> transform)
        => new(((ExpressionExecutionDecoratorBase<TDataTranferObject>)context).With(transform), context.Expression);

    /// <summary>
    /// Replace result transformation logic with custom strategy: convert execution result to target type.
    /// </summary>
    public static ExpressionExecutionContext<TDataTranferObject> With<TDataTranferObject>(
        this ExpressionExecutionContext<TDataTranferObject> context,
        Func<object?, TDataTranferObject> transform)
        => new(((ExpressionExecutionDecoratorBase<TDataTranferObject>)context).With(transform), context.Expression);

    /// <summary>
    /// Decorate with custom strategy: process converted result before being returned.
    /// </summary>
    public static ExpressionExecutionContext<TDataTranferObject> With<TDataTranferObject>(
        this ExpressionExecutionContext<TDataTranferObject> context,
        Func<TDataTranferObject, TDataTranferObject> transform)
        => new(((ExpressionExecutionDecoratorBase<TDataTranferObject>)context).With(transform), context.Expression);
}