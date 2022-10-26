// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionVisitors
{
    using Remote.Linq.ExpressionExecution;
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class SystemExpressionReWriter
    {
        /// <summary>
        /// Replace complicated access to <see cref="IRemoteQueryable"/> by simple <see cref="ConstantExpression"/>.
        /// </summary>
        public static Expression SimplifyIncorporationOfRemoteQueryables(this Expression expression)
            => new RemoteQueryableVisitor().Simplify(expression);

        [SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "Owned disposable type serves special purpose")]
        private sealed class RemoteQueryableVisitor : SystemExpressionVisitorBase
        {
            private sealed class ParameterScope : IDisposable
            {
                private readonly ParameterScope? _parent;
                private readonly RemoteQueryableVisitor _visitor;
                private int _count;

                internal ParameterScope(RemoteQueryableVisitor visitor)
                    : this(visitor, null)
                {
                }

                private ParameterScope(RemoteQueryableVisitor visitor, ParameterScope? parent)
                {
                    _parent = parent;
                    _count = parent?._count ?? 0;

                    _visitor = visitor.CheckNotNull(nameof(visitor));
                    _visitor._parameterScope = this;
                }

                internal void Increment() => _count++;

                void IDisposable.Dispose() => _visitor._parameterScope = _parent!;

                internal bool HasParameters => _count > 0;

                internal IDisposable NewScope() => new ParameterScope(_visitor, this);
            }

            private ParameterScope _parameterScope;

            public RemoteQueryableVisitor()
                => _parameterScope = new ParameterScope(this);

            public Expression Simplify(Expression expression) => Visit(expression);

            protected override Expression VisitMemberAccess(MemberExpression node)
            {
                using (_parameterScope.NewScope())
                {
                    node = (MemberExpression)base.VisitMemberAccess(node);

                    if (!_parameterScope.HasParameters && typeof(IQueryable).IsAssignableFrom(node.Type))
                    {
                        var value = node.CompileAndInvokeExpression();
                        if (value is IRemoteLinqQueryable remoteLinqQueryable)
                        {
                            return remoteLinqQueryable.Expression;
                        }
                    }

                    return node;
                }
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                _parameterScope.Increment();
                return base.VisitParameter(node);
            }
        }
    }
}