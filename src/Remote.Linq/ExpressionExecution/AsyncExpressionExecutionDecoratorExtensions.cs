// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution;

using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using RemoteLinq = Remote.Linq.Expressions;
using SystemLinq = System.Linq.Expressions;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class AsyncExpressionExecutionDecoratorExtensions
{
    /// <summary>
    /// Decorate with custom strategy.
    /// </summary>
    public static AsyncExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this AsyncExpressionExecutionDecorator<TDataTranferObject> decorator,
        Func<RemoteLinq.Expression, RemoteLinq.Expression> transform)
        => new AsyncExpressionExecutorWithRemoteExpressionTransformer<TDataTranferObject>(decorator, transform);

    /// <summary>
    /// Replace expression transformtion logic with custom strategy.
    /// </summary>
    public static AsyncExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this AsyncExpressionExecutionDecorator<TDataTranferObject> decorator,
        Func<RemoteLinq.Expression, SystemLinq.Expression> transform)
        => new AsyncExpressionExecutorWithExpressionTransformer<TDataTranferObject>(decorator, transform);

    /// <summary>
    /// Decorate with custom strategy.
    /// </summary>
    public static AsyncExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this AsyncExpressionExecutionDecorator<TDataTranferObject> decorator,
        Func<SystemLinq.Expression, SystemLinq.Expression> transform)
        => new AsyncExpressionExecutorWithSystemExpressionTransformer<TDataTranferObject>(decorator, transform);

    /// <summary>
    /// Decorate with custom strategy.
    /// </summary>
    public static AsyncExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this AsyncExpressionExecutionDecorator<TDataTranferObject> decorator,
        Func<SystemLinq.Expression, CancellationToken, SystemLinq.Expression> transform)
        => new AsyncExpressionExecutorWithAsyncSystemExpressionTransformer<TDataTranferObject>(decorator, transform);

    /// <summary>
    /// Replace expression execution logic with custom strategy.
    /// </summary>
    public static AsyncExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this AsyncExpressionExecutionDecorator<TDataTranferObject> decorator,
        Func<SystemLinq.Expression, object?> transform)
        => new AsyncExpressionExecutorWithExpressionExecutor<TDataTranferObject>(decorator, transform);

    /// <summary>
    /// Replace expression execution logic with custom strategy.
    /// </summary>
    public static AsyncExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this AsyncExpressionExecutionDecorator<TDataTranferObject> decorator,
        Func<SystemLinq.Expression, CancellationToken, ValueTask<object?>> transform)
        => new AsyncExpressionExecutorWithAsyncExpressionExecutor<TDataTranferObject>(decorator, transform);

    /// <summary>
    /// Decorate with custom strategy.
    /// </summary>
    public static AsyncExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this AsyncExpressionExecutionDecorator<TDataTranferObject> decorator,
        Func<object?, object?> transform)
        => new AsyncExpressionExecutorWithObjectResultProcessor<TDataTranferObject>(decorator, transform);

    /// <summary>
    /// Replace result transformation logic with custom strategy.
    /// </summary>
    public static AsyncExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this AsyncExpressionExecutionDecorator<TDataTranferObject> decorator,
        Func<object?, TDataTranferObject> transform)
        => new AsyncExpressionExecutorWithResultConverter<TDataTranferObject>(decorator, transform);

    /// <summary>
    /// Decorate with custom strategy.
    /// </summary>
    public static AsyncExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this AsyncExpressionExecutionDecorator<TDataTranferObject> decorator,
        Func<TDataTranferObject, TDataTranferObject> transform)
        => new AsyncExpressionExecutorWithDynamicObjectResultProcessor<TDataTranferObject>(decorator, transform);

    /// <summary>
    /// Decorate with custom strategy.
    /// </summary>
    public static AsyncExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this AsyncExpressionExecutor<IQueryable, TDataTranferObject> executor,
        Func<RemoteLinq.Expression, RemoteLinq.Expression> transform)
        => new AsyncExpressionExecutorWithRemoteExpressionTransformer<TDataTranferObject>(executor, transform);

    /// <summary>
    /// Replace expression transformtion logic with custom strategy.
    /// </summary>
    public static AsyncExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this AsyncExpressionExecutor<IQueryable, TDataTranferObject> executor,
        Func<RemoteLinq.Expression, SystemLinq.Expression> transform)
        => new AsyncExpressionExecutorWithExpressionTransformer<TDataTranferObject>(executor, transform);

    /// <summary>
    /// Decorate with custom strategy.
    /// </summary>
    public static AsyncExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this AsyncExpressionExecutor<IQueryable, TDataTranferObject> executor,
        Func<SystemLinq.Expression, SystemLinq.Expression> transform)
        => new AsyncExpressionExecutorWithSystemExpressionTransformer<TDataTranferObject>(executor, transform);

    /// <summary>
    /// Decorate with custom strategy.
    /// </summary>
    public static AsyncExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this AsyncExpressionExecutor<IQueryable, TDataTranferObject> executor,
        Func<SystemLinq.Expression, CancellationToken, SystemLinq.Expression> transform)
        => new AsyncExpressionExecutorWithAsyncSystemExpressionTransformer<TDataTranferObject>(executor, transform);

    /// <summary>
    /// Replace expression execution logic with custom strategy.
    /// </summary>
    public static AsyncExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this AsyncExpressionExecutor<IQueryable, TDataTranferObject> executor,
        Func<SystemLinq.Expression, object?> transform)
        => new AsyncExpressionExecutorWithExpressionExecutor<TDataTranferObject>(executor, transform);

    /// <summary>
    /// Replace expression execution logic with custom strategy.
    /// </summary>
    public static AsyncExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this AsyncExpressionExecutor<IQueryable, TDataTranferObject> executor,
        Func<SystemLinq.Expression, CancellationToken, ValueTask<object?>> transform)
        => new AsyncExpressionExecutorWithAsyncExpressionExecutor<TDataTranferObject>(executor, transform);

    /// <summary>
    /// Decorate with custom strategy.
    /// </summary>
    public static AsyncExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this AsyncExpressionExecutor<IQueryable, TDataTranferObject> executor,
        Func<object?, object?> transform)
        => new AsyncExpressionExecutorWithObjectResultProcessor<TDataTranferObject>(executor, transform);

    /// <summary>
    /// Replace result transformation logic with custom strategy.
    /// </summary>
    public static AsyncExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this AsyncExpressionExecutor<IQueryable, TDataTranferObject> executor,
        Func<object?, TDataTranferObject> transform)
        => new AsyncExpressionExecutorWithResultConverter<TDataTranferObject>(executor, transform);

    /// <summary>
    /// Decorate with custom strategy.
    /// </summary>
    public static AsyncExpressionExecutionDecorator<TDataTranferObject> With<TDataTranferObject>(
        this AsyncExpressionExecutor<IQueryable, TDataTranferObject> executor,
        Func<TDataTranferObject, TDataTranferObject> transform)
        => new AsyncExpressionExecutorWithDynamicObjectResultProcessor<TDataTranferObject>(executor, transform);

    private sealed class AsyncExpressionExecutorWithRemoteExpressionTransformer<TDataTranferObject> : AsyncExpressionExecutionDecorator<TDataTranferObject>
    {
        private readonly Func<RemoteLinq.Expression, RemoteLinq.Expression> _transform;

        public AsyncExpressionExecutorWithRemoteExpressionTransformer(IAsyncExpressionExecutionDecorator<TDataTranferObject> parent, Func<RemoteLinq.Expression, RemoteLinq.Expression> transform)
            : base(parent)
            => _transform = transform.CheckNotNull();

        protected override RemoteLinq.Expression Prepare(RemoteLinq.Expression expression)
            => _transform(base.Prepare(expression));
    }

    private sealed class AsyncExpressionExecutorWithSystemExpressionTransformer<TDataTranferObject> : AsyncExpressionExecutionDecorator<TDataTranferObject>
    {
        private readonly Func<SystemLinq.Expression, SystemLinq.Expression> _transform;

        public AsyncExpressionExecutorWithSystemExpressionTransformer(IAsyncExpressionExecutionDecorator<TDataTranferObject> parent, Func<SystemLinq.Expression, SystemLinq.Expression> transform)
            : base(parent)
            => _transform = transform.CheckNotNull();

        protected override SystemLinq.Expression Prepare(SystemLinq.Expression expression)
            => _transform(base.Prepare(expression));
    }

    private sealed class AsyncExpressionExecutorWithAsyncSystemExpressionTransformer<TDataTranferObject> : AsyncExpressionExecutionDecorator<TDataTranferObject>
    {
        private readonly Func<SystemLinq.Expression, CancellationToken, SystemLinq.Expression> _transform;

        public AsyncExpressionExecutorWithAsyncSystemExpressionTransformer(IAsyncExpressionExecutionDecorator<TDataTranferObject> parent, Func<SystemLinq.Expression, CancellationToken, SystemLinq.Expression> transform)
            : base(parent)
            => _transform = transform.CheckNotNull();

        protected override Expression PrepareAsyncQuery(Expression expression, CancellationToken cancellation)
            => _transform(base.PrepareAsyncQuery(expression, cancellation), cancellation);
    }

    private sealed class AsyncExpressionExecutorWithExpressionTransformer<TDataTranferObject> : AsyncExpressionExecutionDecorator<TDataTranferObject>
    {
        private readonly Func<RemoteLinq.Expression, SystemLinq.Expression> _transform;

        public AsyncExpressionExecutorWithExpressionTransformer(IAsyncExpressionExecutionDecorator<TDataTranferObject> parent, Func<RemoteLinq.Expression, SystemLinq.Expression> transform)
            : base(parent)
            => _transform = transform.CheckNotNull();

        protected override SystemLinq.Expression Transform(RemoteLinq.Expression expression)
            => _transform(expression);
    }

    private sealed class AsyncExpressionExecutorWithExpressionExecutor<TDataTranferObject> : AsyncExpressionExecutionDecorator<TDataTranferObject>
    {
        private readonly Func<SystemLinq.Expression, object?> _transform;

        public AsyncExpressionExecutorWithExpressionExecutor(IAsyncExpressionExecutionDecorator<TDataTranferObject> parent, Func<SystemLinq.Expression, object?> transform)
            : base(parent)
            => _transform = transform.CheckNotNull();

        protected override object? Execute(SystemLinq.Expression expression)
            => _transform(expression);
    }

    private sealed class AsyncExpressionExecutorWithAsyncExpressionExecutor<TDataTranferObject> : AsyncExpressionExecutionDecorator<TDataTranferObject>
    {
        private readonly Func<SystemLinq.Expression, CancellationToken, ValueTask<object?>> _transform;

        public AsyncExpressionExecutorWithAsyncExpressionExecutor(IAsyncExpressionExecutionDecorator<TDataTranferObject> parent, Func<SystemLinq.Expression, CancellationToken, ValueTask<object?>> transform)
            : base(parent)
            => _transform = transform.CheckNotNull();

        protected override ValueTask<object?> ExecuteAsync(SystemLinq.Expression expression, CancellationToken cancellation)
            => _transform(expression, cancellation);
    }

    private sealed class AsyncExpressionExecutorWithObjectResultProcessor<TDataTranferObject> : AsyncExpressionExecutionDecorator<TDataTranferObject>
    {
        private readonly Func<object?, object?> _transform;

        public AsyncExpressionExecutorWithObjectResultProcessor(IAsyncExpressionExecutionDecorator<TDataTranferObject> parent, Func<object?, object?> transform)
            : base(parent)
            => _transform = transform.CheckNotNull();

        protected override object? ProcessResult(object? queryResult)
            => _transform(base.ProcessResult(queryResult));
    }

    private sealed class AsyncExpressionExecutorWithResultConverter<TDataTranferObject> : AsyncExpressionExecutionDecorator<TDataTranferObject>
    {
        private readonly Func<object?, TDataTranferObject> _transform;

        public AsyncExpressionExecutorWithResultConverter(IAsyncExpressionExecutionDecorator<TDataTranferObject> parent, Func<object?, TDataTranferObject> transform)
            : base(parent)
            => _transform = transform.CheckNotNull();

        protected override TDataTranferObject ConvertResult(object? queryResult)
            => _transform(queryResult);
    }

    private sealed class AsyncExpressionExecutorWithDynamicObjectResultProcessor<TDataTranferObject> : AsyncExpressionExecutionDecorator<TDataTranferObject>
    {
        private readonly Func<TDataTranferObject, TDataTranferObject> _transform;

        public AsyncExpressionExecutorWithDynamicObjectResultProcessor(IAsyncExpressionExecutionDecorator<TDataTranferObject> parent, Func<TDataTranferObject, TDataTranferObject> transform)
            : base(parent)
            => _transform = transform.CheckNotNull();

        protected override TDataTranferObject ProcessConvertedResult(TDataTranferObject queryResult)
            => _transform(base.ProcessConvertedResult(queryResult));
    }
}