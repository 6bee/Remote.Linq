// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution;

using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using RemoteLinq = Remote.Linq.Expressions;
using SystemLinq = System.Linq.Expressions;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class AsyncExpressionExecutionContextExtensions
{
    /// <summary>
    /// Decorate with custom strategy.
    /// </summary>
    public static AsyncExpressionExecutionContext<TDataTranferObject> With<TDataTranferObject>(
        this AsyncExpressionExecutionContext<TDataTranferObject> context,
        Func<RemoteLinq.Expression, RemoteLinq.Expression> transform)
        => new(((AsyncExpressionExecutionDecorator<TDataTranferObject>)context).With(transform), context.Expression);

    /// <summary>
    /// Replace expression transformtion logic with custom strategy.
    /// </summary>
    public static AsyncExpressionExecutionContext<TDataTranferObject> With<TDataTranferObject>(
        this AsyncExpressionExecutionContext<TDataTranferObject> context,
        Func<RemoteLinq.Expression, SystemLinq.Expression> transform)
        => new(((AsyncExpressionExecutionDecorator<TDataTranferObject>)context).With(transform), context.Expression);

    /// <summary>
    /// Decorate with custom strategy.
    /// </summary>
    public static AsyncExpressionExecutionContext<TDataTranferObject> With<TDataTranferObject>(
        this AsyncExpressionExecutionContext<TDataTranferObject> context,
        Func<SystemLinq.Expression, SystemLinq.Expression> transform)
        => new(((AsyncExpressionExecutionDecorator<TDataTranferObject>)context).With(transform), context.Expression);

    /// <summary>
    /// Decorate with custom strategy.
    /// </summary>
    public static AsyncExpressionExecutionContext<TDataTranferObject> With<TDataTranferObject>(
        this AsyncExpressionExecutionContext<TDataTranferObject> context,
        Func<SystemLinq.Expression, CancellationToken, SystemLinq.Expression> transform)
        => new(((AsyncExpressionExecutionDecorator<TDataTranferObject>)context).With(transform), context.Expression);

    /// <summary>
    /// Replace expression execution logic with custom strategy.
    /// </summary>
    public static AsyncExpressionExecutionContext<TDataTranferObject> With<TDataTranferObject>(
        this AsyncExpressionExecutionContext<TDataTranferObject> context,
        Func<SystemLinq.Expression, object?> transform)
        => new(((AsyncExpressionExecutionDecorator<TDataTranferObject>)context).With(transform), context.Expression);

    /// <summary>
    /// Replace expression execution logic with custom strategy.
    /// </summary>
    public static AsyncExpressionExecutionContext<TDataTranferObject> With<TDataTranferObject>(
        this AsyncExpressionExecutionContext<TDataTranferObject> context,
        Func<SystemLinq.Expression, CancellationToken, ValueTask<object?>> transform)
        => new(((AsyncExpressionExecutionDecorator<TDataTranferObject>)context).With(transform), context.Expression);

    /// <summary>
    /// Decorate with custom strategy.
    /// </summary>
    public static AsyncExpressionExecutionContext<TDataTranferObject> With<TDataTranferObject>(
        this AsyncExpressionExecutionContext<TDataTranferObject> context,
        Func<object?, object?> transform)
        => new(((AsyncExpressionExecutionDecorator<TDataTranferObject>)context).With(transform), context.Expression);

    /// <summary>
    /// Replace result transformation logic with custom strategy.
    /// </summary>
    public static AsyncExpressionExecutionContext<TDataTranferObject> With<TDataTranferObject>(
        this AsyncExpressionExecutionContext<TDataTranferObject> context,
        Func<object?, TDataTranferObject> transform)
        => new(((AsyncExpressionExecutionDecorator<TDataTranferObject>)context).With(transform), context.Expression);

    /// <summary>
    /// Decorate with custom strategy.
    /// </summary>
    public static AsyncExpressionExecutionContext<TDataTranferObject> With<TDataTranferObject>(
        this AsyncExpressionExecutionContext<TDataTranferObject> context,
        Func<TDataTranferObject, TDataTranferObject> transform)
        => new(((AsyncExpressionExecutionDecorator<TDataTranferObject>)context).With(transform), context.Expression);
}