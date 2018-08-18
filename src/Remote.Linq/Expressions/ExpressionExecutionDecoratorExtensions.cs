// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.Dynamic;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ExpressionExecutionDecoratorExtensions
    {
        /// <summary>
        /// Decorate with custom strategy.
        /// </summary>
        public static ExpressionExecutionDecorator With(this ExpressionExecutionDecorator decorator, Func<Expression, Expression> transform)
            => new ExpressionExecutorWithRemoteExpressionTransformer(decorator, transform);

        /// <summary>
        /// Replace expression transformtion logic with custom strategy.
        /// </summary>
        public static ExpressionExecutionDecorator With(this ExpressionExecutionDecorator decorator, Func<Expression, System.Linq.Expressions.Expression> transform)
            => new ExpressionExecutorWithExpressionTransformer(decorator, transform);

        /// <summary>
        /// Decorate with custom strategy.
        /// </summary>
        public static ExpressionExecutionDecorator With(this ExpressionExecutionDecorator decorator, Func<System.Linq.Expressions.Expression, System.Linq.Expressions.Expression> transform)
            => new ExpressionExecutorWithSystemExpressionTransformer(decorator, transform);

        /// <summary>
        /// Replace expression execution logic with custom strategy.
        /// </summary>
        public static ExpressionExecutionDecorator With(this ExpressionExecutionDecorator decorator, Func<System.Linq.Expressions.Expression, object> transform)
            => new ExpressionExecutorWithExpressionExecutor(decorator, transform);

        /// <summary>
        /// Decorate with custom strategy.
        /// </summary>
        public static ExpressionExecutionDecorator With(this ExpressionExecutionDecorator decorator, Func<object, object> transform)
            => new ExpressionExecutorWithObjectResultProcessor(decorator, transform);

        /// <summary>
        /// Replace result transformation logic with custom strategy.
        /// </summary>
        public static ExpressionExecutionDecorator With(this ExpressionExecutionDecorator decorator, Func<object, IEnumerable<DynamicObject>> transform)
            => new ExpressionExecutorWithResultConverter(decorator, transform);

        /// <summary>
        /// Decorate with custom strategy.
        /// </summary>
        public static ExpressionExecutionDecorator With(this ExpressionExecutionDecorator decorator, Func<IEnumerable<DynamicObject>, IEnumerable<DynamicObject>> transform)
            => new ExpressionExecutorWithDynamicObjectResultProcessor(decorator, transform);

        /// <summary>
        /// Decorate with custom strategy.
        /// </summary>
        public static ExpressionExecutionDecorator With(this ExpressionExecutor executor, Func<Expression, Expression> transform)
            => new ExpressionExecutorWithRemoteExpressionTransformer(executor, transform);

        /// <summary>
        /// Replace expression transformtion logic with custom strategy.
        /// </summary>
        public static ExpressionExecutionDecorator With(this ExpressionExecutor executor, Func<Expression, System.Linq.Expressions.Expression> transform)
            => new ExpressionExecutorWithExpressionTransformer(executor, transform);

        /// <summary>
        /// Decorate with custom strategy.
        /// </summary>
        public static ExpressionExecutionDecorator With(this ExpressionExecutor executor, Func<System.Linq.Expressions.Expression, System.Linq.Expressions.Expression> transform)
            => new ExpressionExecutorWithSystemExpressionTransformer(executor, transform);

        /// <summary>
        /// Replace expression execution logic with custom strategy.
        /// </summary>
        public static ExpressionExecutionDecorator With(this ExpressionExecutor executor, Func<System.Linq.Expressions.Expression, object> transform)
            => new ExpressionExecutorWithExpressionExecutor(executor, transform);

        /// <summary>
        /// Decorate with custom strategy.
        /// </summary>
        public static ExpressionExecutionDecorator With(this ExpressionExecutor executor, Func<object, object> transform)
            => new ExpressionExecutorWithObjectResultProcessor(executor, transform);

        /// <summary>
        /// Replace result transformation logic with custom strategy.
        /// </summary>
        public static ExpressionExecutionDecorator With(this ExpressionExecutor executor, Func<object, IEnumerable<DynamicObject>> transform)
            => new ExpressionExecutorWithResultConverter(executor, transform);

        /// <summary>
        /// Decorate with custom strategy.
        /// </summary>
        public static ExpressionExecutionDecorator With(this ExpressionExecutor executor, Func<IEnumerable<DynamicObject>, IEnumerable<DynamicObject>> transform)
            => new ExpressionExecutorWithDynamicObjectResultProcessor(executor, transform);

        private sealed class ExpressionExecutorWithRemoteExpressionTransformer : ExpressionExecutionDecorator
        {
            private readonly Func<Expression, Expression> _transform;

            public ExpressionExecutorWithRemoteExpressionTransformer(IExpressionExecutionDecorator parent, Func<Expression, Expression> transform)
                : base(parent)
            {
                _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            }

            protected override Expression Prepare(Expression expression)
                => _transform(base.Prepare(expression));
        }

        private sealed class ExpressionExecutorWithSystemExpressionTransformer : ExpressionExecutionDecorator
        {
            private readonly Func<System.Linq.Expressions.Expression, System.Linq.Expressions.Expression> _transform;

            public ExpressionExecutorWithSystemExpressionTransformer(IExpressionExecutionDecorator parent, Func<System.Linq.Expressions.Expression, System.Linq.Expressions.Expression> transform)
                : base(parent)
            {
                _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            }

            protected override System.Linq.Expressions.Expression Prepare(System.Linq.Expressions.Expression expression)
                => _transform(base.Prepare(expression));
        }

        private sealed class ExpressionExecutorWithExpressionTransformer : ExpressionExecutionDecorator
        {
            private readonly Func<Expression, System.Linq.Expressions.Expression> _transform;

            public ExpressionExecutorWithExpressionTransformer(IExpressionExecutionDecorator parent, Func<Expression, System.Linq.Expressions.Expression> transform)
                : base(parent)
            {
                _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            }

            protected override System.Linq.Expressions.Expression Transform(Expression expression)
                => _transform(expression);
        }

        private sealed class ExpressionExecutorWithExpressionExecutor : ExpressionExecutionDecorator
        {
            private readonly Func<System.Linq.Expressions.Expression, object> _transform;

            public ExpressionExecutorWithExpressionExecutor(IExpressionExecutionDecorator parent, Func<System.Linq.Expressions.Expression, object> transform)
                : base(parent)
            {
                _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            }

            protected override object Execute(System.Linq.Expressions.Expression expression)
                => _transform(expression);
        }

        private sealed class ExpressionExecutorWithObjectResultProcessor : ExpressionExecutionDecorator
        {
            private readonly Func<object, object> _transform;

            public ExpressionExecutorWithObjectResultProcessor(IExpressionExecutionDecorator parent, Func<object, object> transform)
                : base(parent)
            {
                _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            }

            protected override object ProcessResult(object queryResult)
                => _transform(base.ProcessResult(queryResult));
        }

        private sealed class ExpressionExecutorWithResultConverter : ExpressionExecutionDecorator
        {
            private readonly Func<object, IEnumerable<DynamicObject>> _transform;

            public ExpressionExecutorWithResultConverter(IExpressionExecutionDecorator parent, Func<object, IEnumerable<DynamicObject>> transform)
                : base(parent)
            {
                _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            }

            protected override IEnumerable<DynamicObject> ConvertResult(object queryResult)
                => _transform(queryResult);
        }

        private sealed class ExpressionExecutorWithDynamicObjectResultProcessor : ExpressionExecutionDecorator
        {
            private readonly Func<IEnumerable<DynamicObject>, IEnumerable<DynamicObject>> _transform;

            public ExpressionExecutorWithDynamicObjectResultProcessor(IExpressionExecutionDecorator parent, Func<IEnumerable<DynamicObject>, IEnumerable<DynamicObject>> transform)
                : base(parent)
            {
                _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            }

            protected override IEnumerable<DynamicObject> ProcessResult(IEnumerable<DynamicObject> queryResult)
                => _transform(base.ProcessResult(queryResult));
        }
    }
}
