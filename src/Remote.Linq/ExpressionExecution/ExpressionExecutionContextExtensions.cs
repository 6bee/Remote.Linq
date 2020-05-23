// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution
{
    using Remote.Linq.Expressions;
    using System;
    using System.ComponentModel;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ExpressionExecutionContextExtensions
    {
        /// <summary>
        /// Decorate with custom strategy.
        /// </summary>
        public static ExpressionExecutionContext<TDataTranferObject> With<TDataTranferObject>(this ExpressionExecutionContext<TDataTranferObject> context, Func<Expression, Expression> transform)
            => new ExpressionExecutionContextWithRemoteExpressionTransformer<TDataTranferObject>(context, transform);

        /// <summary>
        /// Replace expression transformtion logic with custom strategy.
        /// </summary>
        public static ExpressionExecutionContext<TDataTranferObject> With<TDataTranferObject>(this ExpressionExecutionContext<TDataTranferObject> context, Func<Expression, System.Linq.Expressions.Expression> transform)
            => new ExpressionExecutionContextWithExpressionTransformer<TDataTranferObject>(context, transform);

        /// <summary>
        /// Decorate with custom strategy.
        /// </summary>
        public static ExpressionExecutionContext<TDataTranferObject> With<TDataTranferObject>(this ExpressionExecutionContext<TDataTranferObject> context, Func<System.Linq.Expressions.Expression, System.Linq.Expressions.Expression> transform)
            => new ExpressionExecutionContextWithSystemExpressionTransformer<TDataTranferObject>(context, transform);

        /// <summary>
        /// Replace expression execution logic with custom strategy.
        /// </summary>
        public static ExpressionExecutionContext<TDataTranferObject> With<TDataTranferObject>(this ExpressionExecutionContext<TDataTranferObject> context, Func<System.Linq.Expressions.Expression, object?> transform)
            => new ExpressionExecutionContextWithExpressionExecutor<TDataTranferObject>(context, transform);

        /// <summary>
        /// Decorate with custom strategy.
        /// </summary>
        public static ExpressionExecutionContext<TDataTranferObject> With<TDataTranferObject>(this ExpressionExecutionContext<TDataTranferObject> context, Func<object?, object?> transform)
            => new ExpressionExecutionContextWithObjectResultProcessor<TDataTranferObject>(context, transform);

        /// <summary>
        /// Replace result transformation logic with custom strategy.
        /// </summary>
        public static ExpressionExecutionContext<TDataTranferObject> With<TDataTranferObject>(this ExpressionExecutionContext<TDataTranferObject> context, Func<object?, TDataTranferObject> transform)
            => new ExpressionExecutionContextWithResultConverter<TDataTranferObject>(context, transform);

        /// <summary>
        /// Decorate with custom strategy.
        /// </summary>
        public static ExpressionExecutionContext<TDataTranferObject> With<TDataTranferObject>(this ExpressionExecutionContext<TDataTranferObject> context, Func<TDataTranferObject, TDataTranferObject> transform)
            => new ExpressionExecutionContextWithDynamicObjectResultProcessor<TDataTranferObject>(context, transform);

        private sealed class ExpressionExecutionContextWithRemoteExpressionTransformer<TDataTranferObject> : ExpressionExecutionContext<TDataTranferObject>
        {
            private readonly Func<Expression, Expression> _transform;

            public ExpressionExecutionContextWithRemoteExpressionTransformer(ExpressionExecutionContext<TDataTranferObject> parent, Func<Expression, Expression> transform)
                : base(parent)
            {
                _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            }

            protected override Expression Prepare(Expression expression)
                => _transform(base.Prepare(expression));
        }

        private sealed class ExpressionExecutionContextWithSystemExpressionTransformer<TDataTranferObject> : ExpressionExecutionContext<TDataTranferObject>
        {
            private readonly Func<System.Linq.Expressions.Expression, System.Linq.Expressions.Expression> _transform;

            public ExpressionExecutionContextWithSystemExpressionTransformer(ExpressionExecutionContext<TDataTranferObject> parent, Func<System.Linq.Expressions.Expression, System.Linq.Expressions.Expression> transform)
                : base(parent)
            {
                _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            }

            protected override System.Linq.Expressions.Expression Prepare(System.Linq.Expressions.Expression expression)
                => _transform(base.Prepare(expression));
        }

        private sealed class ExpressionExecutionContextWithExpressionTransformer<TDataTranferObject> : ExpressionExecutionContext<TDataTranferObject>
        {
            private readonly Func<Expression, System.Linq.Expressions.Expression> _transform;

            public ExpressionExecutionContextWithExpressionTransformer(ExpressionExecutionContext<TDataTranferObject> parent, Func<Expression, System.Linq.Expressions.Expression> transform)
                : base(parent)
            {
                _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            }

            protected override System.Linq.Expressions.Expression Transform(Expression expression)
                => _transform(expression);
        }

        private sealed class ExpressionExecutionContextWithExpressionExecutor<TDataTranferObject> : ExpressionExecutionContext<TDataTranferObject>
        {
            private readonly Func<System.Linq.Expressions.Expression, object?> _transform;

            public ExpressionExecutionContextWithExpressionExecutor(ExpressionExecutionContext<TDataTranferObject> parent, Func<System.Linq.Expressions.Expression, object?> transform)
                : base(parent)
            {
                _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            }

            protected override object? Execute(System.Linq.Expressions.Expression expression)
                => _transform(expression);
        }

        private sealed class ExpressionExecutionContextWithObjectResultProcessor<TDataTranferObject> : ExpressionExecutionContext<TDataTranferObject>
        {
            private readonly Func<object?, object?> _transform;

            public ExpressionExecutionContextWithObjectResultProcessor(ExpressionExecutionContext<TDataTranferObject> parent, Func<object?, object?> transform)
                : base(parent)
            {
                _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            }

            protected override object? ProcessResult(object? queryResult)
                => _transform(base.ProcessResult(queryResult));
        }

        private sealed class ExpressionExecutionContextWithResultConverter<TDataTranferObject> : ExpressionExecutionContext<TDataTranferObject>
        {
            private readonly Func<object?, TDataTranferObject> _transform;

            public ExpressionExecutionContextWithResultConverter(ExpressionExecutionContext<TDataTranferObject> parent, Func<object?, TDataTranferObject> transform)
                : base(parent)
            {
                _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            }

            protected override TDataTranferObject ConvertResult(object? queryResult)
                => _transform(queryResult);
        }

        private sealed class ExpressionExecutionContextWithDynamicObjectResultProcessor<TDataTranferObject> : ExpressionExecutionContext<TDataTranferObject>
        {
            private readonly Func<TDataTranferObject, TDataTranferObject> _transform;

            public ExpressionExecutionContextWithDynamicObjectResultProcessor(ExpressionExecutionContext<TDataTranferObject> parent, Func<TDataTranferObject, TDataTranferObject> transform)
                : base(parent)
            {
                _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            }

            protected override TDataTranferObject ProcessResult(TDataTranferObject queryResult)
                => _transform(base.ProcessResult(queryResult));
        }
    }
}
