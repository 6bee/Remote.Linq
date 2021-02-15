// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution
{
    using System;
    using System.ComponentModel;
    using RemoteLinq = Remote.Linq.Expressions;
    using SystemLinq = System.Linq.Expressions;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ExpressionExecutionContextExtensions
    {
        /// <summary>
        /// Decorate with custom strategy.
        /// </summary>
        public static ExpressionExecutionContext<TDataTranferObject> With<TDataTranferObject>(
            this ExpressionExecutionContext<TDataTranferObject> context,
            Func<RemoteLinq.Expression, RemoteLinq.Expression> transform)
            => new ExpressionExecutionContextWithRemoteExpressionTransformer<TDataTranferObject>(context, transform);

        /// <summary>
        /// Replace expression transformtion logic with custom strategy.
        /// </summary>
        public static ExpressionExecutionContext<TDataTranferObject> With<TDataTranferObject>(
            this ExpressionExecutionContext<TDataTranferObject> context,
            Func<RemoteLinq.Expression, SystemLinq.Expression> transform)
            => new ExpressionExecutionContextWithExpressionTransformer<TDataTranferObject>(context, transform);

        /// <summary>
        /// Decorate with custom strategy.
        /// </summary>
        public static ExpressionExecutionContext<TDataTranferObject> With<TDataTranferObject>(
            this ExpressionExecutionContext<TDataTranferObject> context,
            Func<SystemLinq.Expression, SystemLinq.Expression> transform)
            => new ExpressionExecutionContextWithSystemExpressionTransformer<TDataTranferObject>(context, transform);

        /// <summary>
        /// Replace expression execution logic with custom strategy.
        /// </summary>
        public static ExpressionExecutionContext<TDataTranferObject> With<TDataTranferObject>(
            this ExpressionExecutionContext<TDataTranferObject> context,
            Func<SystemLinq.Expression, object?> transform)
            => new ExpressionExecutionContextWithExpressionExecutor<TDataTranferObject>(context, transform);

        /// <summary>
        /// Decorate with custom strategy.
        /// </summary>
        public static ExpressionExecutionContext<TDataTranferObject> With<TDataTranferObject>(
            this ExpressionExecutionContext<TDataTranferObject> context,
            Func<object?, object?> transform)
            => new ExpressionExecutionContextWithObjectResultProcessor<TDataTranferObject>(context, transform);

        /// <summary>
        /// Replace result transformation logic with custom strategy.
        /// </summary>
        public static ExpressionExecutionContext<TDataTranferObject> With<TDataTranferObject>(
            this ExpressionExecutionContext<TDataTranferObject> context,
            Func<object?, TDataTranferObject> transform)
            => new ExpressionExecutionContextWithResultConverter<TDataTranferObject>(context, transform);

        /// <summary>
        /// Decorate with custom strategy.
        /// </summary>
        public static ExpressionExecutionContext<TDataTranferObject> With<TDataTranferObject>(
            this ExpressionExecutionContext<TDataTranferObject> context,
            Func<TDataTranferObject, TDataTranferObject> transform)
            => new ExpressionExecutionContextWithDynamicObjectResultProcessor<TDataTranferObject>(context, transform);

        private sealed class ExpressionExecutionContextWithRemoteExpressionTransformer<TDataTranferObject> : ExpressionExecutionContext<TDataTranferObject>
        {
            private readonly Func<RemoteLinq.Expression, RemoteLinq.Expression> _transform;

            public ExpressionExecutionContextWithRemoteExpressionTransformer(ExpressionExecutionContext<TDataTranferObject> parent, Func<RemoteLinq.Expression, RemoteLinq.Expression> transform)
                : base(parent)
                => _transform = transform.CheckNotNull(nameof(transform));

            protected override RemoteLinq.Expression Prepare(RemoteLinq.Expression expression)
                => _transform(base.Prepare(expression));
        }

        private sealed class ExpressionExecutionContextWithSystemExpressionTransformer<TDataTranferObject> : ExpressionExecutionContext<TDataTranferObject>
        {
            private readonly Func<SystemLinq.Expression, SystemLinq.Expression> _transform;

            public ExpressionExecutionContextWithSystemExpressionTransformer(ExpressionExecutionContext<TDataTranferObject> parent, Func<SystemLinq.Expression, SystemLinq.Expression> transform)
                : base(parent)
                => _transform = transform.CheckNotNull(nameof(transform));

            protected override SystemLinq.Expression Prepare(SystemLinq.Expression expression)
                => _transform(base.Prepare(expression));
        }

        private sealed class ExpressionExecutionContextWithExpressionTransformer<TDataTranferObject> : ExpressionExecutionContext<TDataTranferObject>
        {
            private readonly Func<RemoteLinq.Expression, SystemLinq.Expression> _transform;

            public ExpressionExecutionContextWithExpressionTransformer(ExpressionExecutionContext<TDataTranferObject> parent, Func<RemoteLinq.Expression, SystemLinq.Expression> transform)
                : base(parent)
                => _transform = transform.CheckNotNull(nameof(transform));

            protected override SystemLinq.Expression Transform(RemoteLinq.Expression expression)
                => _transform(expression);
        }

        private sealed class ExpressionExecutionContextWithExpressionExecutor<TDataTranferObject> : ExpressionExecutionContext<TDataTranferObject>
        {
            private readonly Func<SystemLinq.Expression, object?> _transform;

            public ExpressionExecutionContextWithExpressionExecutor(ExpressionExecutionContext<TDataTranferObject> parent, Func<SystemLinq.Expression, object?> transform)
                : base(parent)
                => _transform = transform.CheckNotNull(nameof(transform));

            protected override object? Execute(SystemLinq.Expression expression)
                => _transform(expression);
        }

        private sealed class ExpressionExecutionContextWithObjectResultProcessor<TDataTranferObject> : ExpressionExecutionContext<TDataTranferObject>
        {
            private readonly Func<object?, object?> _transform;

            public ExpressionExecutionContextWithObjectResultProcessor(ExpressionExecutionContext<TDataTranferObject> parent, Func<object?, object?> transform)
                : base(parent)
                => _transform = transform.CheckNotNull(nameof(transform));

            protected override object? ProcessResult(object? queryResult)
                => _transform(base.ProcessResult(queryResult));
        }

        private sealed class ExpressionExecutionContextWithResultConverter<TDataTranferObject> : ExpressionExecutionContext<TDataTranferObject>
        {
            private readonly Func<object?, TDataTranferObject> _transform;

            public ExpressionExecutionContextWithResultConverter(ExpressionExecutionContext<TDataTranferObject> parent, Func<object?, TDataTranferObject> transform)
                : base(parent)
                => _transform = transform.CheckNotNull(nameof(transform));

            protected override TDataTranferObject ConvertResult(object? queryResult)
                => _transform(queryResult);
        }

        private sealed class ExpressionExecutionContextWithDynamicObjectResultProcessor<TDataTranferObject> : ExpressionExecutionContext<TDataTranferObject>
        {
            private readonly Func<TDataTranferObject, TDataTranferObject> _transform;

            public ExpressionExecutionContextWithDynamicObjectResultProcessor(ExpressionExecutionContext<TDataTranferObject> parent, Func<TDataTranferObject, TDataTranferObject> transform)
                : base(parent)
                => _transform = transform.CheckNotNull(nameof(transform));

            protected override TDataTranferObject ProcessResult(TDataTranferObject queryResult)
                => _transform(base.ProcessResult(queryResult));
        }
    }
}