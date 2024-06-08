// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution;

using System;
using System.ComponentModel;
using System.Linq;
using RemoteLinq = Remote.Linq.Expressions;
using SystemLinq = System.Linq.Expressions;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class ExpressionExecutionDecoratorExtensions
{
    /// <summary>
    /// Decorate with custom strategy: prepare remote expression.
    /// </summary>
    public static ExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this ExpressionExecutionDecoratorBase<TDataTranferObject> decorator,
        Func<RemoteLinq.Expression, RemoteLinq.Expression> transform)
        => new ExpressionExecutorWithRemoteExpressionTransformer<TDataTranferObject>(decorator, transform);

    /// <summary>
    /// Replace expression transformtion logic with custom strategy: transform remote expression to system expression.
    /// </summary>
    public static ExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this ExpressionExecutionDecoratorBase<TDataTranferObject> decorator,
        Func<RemoteLinq.Expression, SystemLinq.Expression> transform)
        => new ExpressionExecutorWithExpressionTransformer<TDataTranferObject>(decorator, transform);

    /// <summary>
    /// Decorate with custom strategy: prepare system expression.
    /// </summary>
    public static ExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this ExpressionExecutionDecoratorBase<TDataTranferObject> decorator,
        Func<SystemLinq.Expression, SystemLinq.Expression> transform)
        => new ExpressionExecutorWithSystemExpressionTransformer<TDataTranferObject>(decorator, transform);

    /// <summary>
    /// Replace expression execution logic with custom strategy: execute expression.
    /// </summary>
    public static ExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this ExpressionExecutionDecoratorBase<TDataTranferObject> decorator,
        Func<SystemLinq.Expression, object?> transform)
        => new ExpressionExecutorWithExpressionExecutor<TDataTranferObject>(decorator, transform);

    /// <summary>
    /// Decorate with custom strategy: process execution result.
    /// </summary>
    public static ExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this ExpressionExecutionDecoratorBase<TDataTranferObject> decorator,
        Func<object?, object?> transform)
        => new ExpressionExecutorWithObjectResultProcessor<TDataTranferObject>(decorator, transform);

    /// <summary>
    /// Replace result transformation logic with custom strategy: convert execution result to target type.
    /// </summary>
    public static ExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this ExpressionExecutionDecoratorBase<TDataTranferObject> decorator,
        Func<object?, TDataTranferObject> transform)
        => new ExpressionExecutorWithResultConverter<TDataTranferObject>(decorator, transform);

    /// <summary>
    /// Decorate with custom strategy: process converted result before being returned.
    /// </summary>
    public static ExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this ExpressionExecutionDecoratorBase<TDataTranferObject> decorator,
        Func<TDataTranferObject, TDataTranferObject> transform)
        => new ExpressionExecutorWithDynamicObjectResultProcessor<TDataTranferObject>(decorator, transform);

    /// <summary>
    /// Decorate with custom strategy: prepare remote expression.
    /// </summary>
    public static ExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this ExpressionExecutor<IQueryable, TDataTranferObject> executor,
        Func<RemoteLinq.Expression, RemoteLinq.Expression> transform)
        => new ExpressionExecutorWithRemoteExpressionTransformer<TDataTranferObject>(executor, transform);

    /// <summary>
    /// Replace expression transformtion logic with custom strategy: transform remote expression to system expression.
    /// </summary>
    public static ExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this ExpressionExecutor<IQueryable, TDataTranferObject> executor,
        Func<RemoteLinq.Expression, SystemLinq.Expression> transform)
        => new ExpressionExecutorWithExpressionTransformer<TDataTranferObject>(executor, transform);

    /// <summary>
    /// Decorate with custom strategy: prepare system expression.
    /// </summary>
    public static ExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this ExpressionExecutor<IQueryable, TDataTranferObject> executor,
        Func<SystemLinq.Expression, SystemLinq.Expression> transform)
        => new ExpressionExecutorWithSystemExpressionTransformer<TDataTranferObject>(executor, transform);

    /// <summary>
    /// Replace expression execution logic with custom strategy: execute expression.
    /// </summary>
    public static ExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this ExpressionExecutor<IQueryable, TDataTranferObject> executor,
        Func<SystemLinq.Expression, object?> transform)
        => new ExpressionExecutorWithExpressionExecutor<TDataTranferObject>(executor, transform);

    /// <summary>
    /// Decorate with custom strategy: process execution result.
    /// </summary>
    public static ExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this ExpressionExecutor<IQueryable, TDataTranferObject> executor,
        Func<object?, object?> transform)
        => new ExpressionExecutorWithObjectResultProcessor<TDataTranferObject>(executor, transform);

    /// <summary>
    /// Replace result transformation logic with custom strategy: convert execution result to target type.
    /// </summary>
    public static ExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this ExpressionExecutor<IQueryable, TDataTranferObject> executor,
        Func<object?, TDataTranferObject> transform)
        => new ExpressionExecutorWithResultConverter<TDataTranferObject>(executor, transform);

    /// <summary>
    /// Decorate with custom strategy: process converted result before being returned.
    /// </summary>
    public static ExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this ExpressionExecutor<IQueryable, TDataTranferObject> executor,
        Func<TDataTranferObject, TDataTranferObject> transform)
        => new ExpressionExecutorWithDynamicObjectResultProcessor<TDataTranferObject>(executor, transform);

    private sealed class ExpressionExecutorWithRemoteExpressionTransformer<TDataTranferObject> : ExpressionExecutionDecorator<TDataTranferObject>
    {
        private readonly Func<RemoteLinq.Expression, RemoteLinq.Expression> _transform;

        public ExpressionExecutorWithRemoteExpressionTransformer(IExpressionExecutionDecorator<TDataTranferObject> parent, Func<RemoteLinq.Expression, RemoteLinq.Expression> transform)
            : base(parent)
            => _transform = transform.CheckNotNull();

        protected override RemoteLinq.Expression Prepare(RemoteLinq.Expression expression)
            => _transform(base.Prepare(expression));
    }

    private sealed class ExpressionExecutorWithSystemExpressionTransformer<TDataTranferObject> : ExpressionExecutionDecorator<TDataTranferObject>
    {
        private readonly Func<SystemLinq.Expression, SystemLinq.Expression> _transform;

        public ExpressionExecutorWithSystemExpressionTransformer(IExpressionExecutionDecorator<TDataTranferObject> parent, Func<SystemLinq.Expression, SystemLinq.Expression> transform)
            : base(parent)
            => _transform = transform.CheckNotNull();

        protected override SystemLinq.Expression Prepare(SystemLinq.Expression expression)
            => _transform(base.Prepare(expression));
    }

    private sealed class ExpressionExecutorWithExpressionTransformer<TDataTranferObject> : ExpressionExecutionDecorator<TDataTranferObject>
    {
        private readonly Func<RemoteLinq.Expression, SystemLinq.Expression> _transform;

        public ExpressionExecutorWithExpressionTransformer(IExpressionExecutionDecorator<TDataTranferObject> parent, Func<RemoteLinq.Expression, SystemLinq.Expression> transform)
            : base(parent)
            => _transform = transform.CheckNotNull();

        protected override SystemLinq.Expression Transform(RemoteLinq.Expression expression)
            => _transform(expression);
    }

    private sealed class ExpressionExecutorWithExpressionExecutor<TDataTranferObject> : ExpressionExecutionDecorator<TDataTranferObject>
    {
        private readonly Func<SystemLinq.Expression, object?> _transform;

        public ExpressionExecutorWithExpressionExecutor(IExpressionExecutionDecorator<TDataTranferObject> parent, Func<SystemLinq.Expression, object?> transform)
            : base(parent)
            => _transform = transform.CheckNotNull();

        protected override object? Execute(SystemLinq.Expression expression)
            => _transform(expression);
    }

    private sealed class ExpressionExecutorWithObjectResultProcessor<TDataTranferObject> : ExpressionExecutionDecorator<TDataTranferObject>
    {
        private readonly Func<object?, object?> _transform;

        public ExpressionExecutorWithObjectResultProcessor(IExpressionExecutionDecorator<TDataTranferObject> parent, Func<object?, object?> transform)
            : base(parent)
            => _transform = transform.CheckNotNull();

        protected override object? ProcessResult(object? queryResult)
            => _transform(base.ProcessResult(queryResult));
    }

    private sealed class ExpressionExecutorWithResultConverter<TDataTranferObject> : ExpressionExecutionDecorator<TDataTranferObject>
    {
        private readonly Func<object?, TDataTranferObject> _transform;

        public ExpressionExecutorWithResultConverter(IExpressionExecutionDecorator<TDataTranferObject> parent, Func<object?, TDataTranferObject> transform)
            : base(parent)
            => _transform = transform.CheckNotNull();

        protected override TDataTranferObject ConvertResult(object? queryResult)
            => _transform(queryResult);
    }

    private sealed class ExpressionExecutorWithDynamicObjectResultProcessor<TDataTranferObject> : ExpressionExecutionDecorator<TDataTranferObject>
    {
        private readonly Func<TDataTranferObject, TDataTranferObject> _transform;

        public ExpressionExecutorWithDynamicObjectResultProcessor(IExpressionExecutionDecorator<TDataTranferObject> parent, Func<TDataTranferObject, TDataTranferObject> transform)
            : base(parent)
            => _transform = transform.CheckNotNull();

        protected override TDataTranferObject ProcessConvertedResult(TDataTranferObject queryResult)
            => _transform(base.ProcessConvertedResult(queryResult));
    }
}