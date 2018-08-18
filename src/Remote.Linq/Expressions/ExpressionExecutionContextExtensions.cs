// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.Dynamic;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ExpressionExecutionContextExtensions
    {
        /// <summary>
        /// Decorate with custom strategy.
        /// </summary>
        public static ExpressionExecutionContext With(this ExpressionExecutionContext context, Func<Expression, Expression> transform)
            => new ExpressionExecutionContextWithRemoteExpressionTransformer(context, transform);

        /// <summary>
        /// Replace expression transformtion logic with custom strategy.
        /// </summary>
        public static ExpressionExecutionContext With(this ExpressionExecutionContext context, Func<Expression, System.Linq.Expressions.Expression> transform)
            => new ExpressionExecutionContextWithExpressionTransformer(context, transform);

        /// <summary>
        /// Decorate with custom strategy.
        /// </summary>
        public static ExpressionExecutionContext With(this ExpressionExecutionContext context, Func<System.Linq.Expressions.Expression, System.Linq.Expressions.Expression> transform)
            => new ExpressionExecutionContextWithSystemExpressionTransformer(context, transform);

        /// <summary>
        /// Replace expression execution logic with custom strategy.
        /// </summary>
        public static ExpressionExecutionContext With(this ExpressionExecutionContext context, Func<System.Linq.Expressions.Expression, object> transform)
            => new ExpressionExecutionContextWithExpressionExecutor(context, transform);

        /// <summary>
        /// Decorate with custom strategy.
        /// </summary>
        public static ExpressionExecutionContext With(this ExpressionExecutionContext context, Func<object, object> transform)
            => new ExpressionExecutionContextWithObjectResultProcessor(context, transform);

        /// <summary>
        /// Replace result transformation logic with custom strategy.
        /// </summary>
        public static ExpressionExecutionContext With(this ExpressionExecutionContext context, Func<object, IEnumerable<DynamicObject>> transform)
            => new ExpressionExecutionContextWithResultConverter(context, transform);

        /// <summary>
        /// Decorate with custom strategy.
        /// </summary>
        public static ExpressionExecutionContext With(this ExpressionExecutionContext context, Func<IEnumerable<DynamicObject>, IEnumerable<DynamicObject>> transform)
            => new ExpressionExecutionContextWithDynamicObjectResultProcessor(context, transform);

        private sealed class ExpressionExecutionContextWithRemoteExpressionTransformer : ExpressionExecutionContext
        {
            private readonly Func<Expression, Expression> _transform;

            public ExpressionExecutionContextWithRemoteExpressionTransformer(ExpressionExecutionContext parent, Func<Expression, Expression> transform)
                : base(parent)
            {
                _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            }

            protected override Expression Prepare(Expression expression)
                => _transform(base.Prepare(expression));
        }

        private sealed class ExpressionExecutionContextWithSystemExpressionTransformer : ExpressionExecutionContext
        {
            private readonly Func<System.Linq.Expressions.Expression, System.Linq.Expressions.Expression> _transform;

            public ExpressionExecutionContextWithSystemExpressionTransformer(ExpressionExecutionContext parent, Func<System.Linq.Expressions.Expression, System.Linq.Expressions.Expression> transform)
                : base(parent)
            {
                _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            }

            protected override System.Linq.Expressions.Expression Prepare(System.Linq.Expressions.Expression expression)
                => _transform(base.Prepare(expression));
        }

        private sealed class ExpressionExecutionContextWithExpressionTransformer : ExpressionExecutionContext
        {
            private readonly Func<Expression, System.Linq.Expressions.Expression> _transform;

            public ExpressionExecutionContextWithExpressionTransformer(ExpressionExecutionContext parent, Func<Expression, System.Linq.Expressions.Expression> transform)
                : base(parent)
            {
                _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            }

            protected override System.Linq.Expressions.Expression Transform(Expression expression)
                => _transform(expression);
        }

        private sealed class ExpressionExecutionContextWithExpressionExecutor : ExpressionExecutionContext
        {
            private readonly Func<System.Linq.Expressions.Expression, object> _transform;

            public ExpressionExecutionContextWithExpressionExecutor(ExpressionExecutionContext parent, Func<System.Linq.Expressions.Expression, object> transform)
                : base(parent)
            {
                _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            }

            protected override object Execute(System.Linq.Expressions.Expression expression)
                => _transform(expression);
        }

        private sealed class ExpressionExecutionContextWithObjectResultProcessor : ExpressionExecutionContext
        {
            private readonly Func<object, object> _transform;

            public ExpressionExecutionContextWithObjectResultProcessor(ExpressionExecutionContext parent, Func<object, object> transform)
                : base(parent)
            {
                _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            }

            protected override object ProcessResult(object queryResult)
                => _transform(base.ProcessResult(queryResult));
        }

        private sealed class ExpressionExecutionContextWithResultConverter : ExpressionExecutionContext
        {
            private readonly Func<object, IEnumerable<DynamicObject>> _transform;

            public ExpressionExecutionContextWithResultConverter(ExpressionExecutionContext parent, Func<object, IEnumerable<DynamicObject>> transform)
                : base(parent)
            {
                _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            }

            protected override IEnumerable<DynamicObject> ConvertResult(object queryResult)
                => _transform(queryResult);
        }

        private sealed class ExpressionExecutionContextWithDynamicObjectResultProcessor : ExpressionExecutionContext
        {
            private readonly Func<IEnumerable<DynamicObject>, IEnumerable<DynamicObject>> _transform;

            public ExpressionExecutionContextWithDynamicObjectResultProcessor(ExpressionExecutionContext parent, Func<IEnumerable<DynamicObject>, IEnumerable<DynamicObject>> transform)
                : base(parent)
            {
                _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            }

            protected override IEnumerable<DynamicObject> ProcessResult(IEnumerable<DynamicObject> queryResult)
                => _transform(base.ProcessResult(queryResult));
        }
    }
}
