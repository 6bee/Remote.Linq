// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Aqua.TypeSystem;
    using System;
    using System.Linq;
    using Expression = System.Linq.Expressions.Expression;
    using MethodInfo = System.Reflection.MethodInfo;

    internal sealed class RemoteQueryProvider<TSource> : IRemoteQueryProvider
    {
        private static readonly MethodInfo _executeMethod = typeof(RemoteQueryProvider<TSource>)
            .GetMethods()
            .Single(x => x.IsGenericMethod && string.Equals(x.Name, nameof(Execute), StringComparison.Ordinal));

        private readonly Func<Expressions.Expression, TSource?> _dataProvider;
        private readonly IQueryResultMapper<TSource> _resultMapper;
        private readonly IExpressionToRemoteLinqContext? _context;

        public RemoteQueryProvider(
            Func<Expressions.Expression, TSource?> dataProvider,
            IQueryResultMapper<TSource> resultMapper,
            IExpressionToRemoteLinqContext? context)
        {
            _dataProvider = dataProvider.CheckNotNull(nameof(dataProvider));
            _resultMapper = resultMapper.CheckNotNull(nameof(resultMapper));
            _context = context;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            => new RemoteQueryable<TElement>(this, expression);

        public IQueryable CreateQuery(Expression expression)
        {
            var elementType = TypeHelper.GetElementType(expression.Type)
                ?? throw new RemoteLinqException($"Cannot get element type based on expression's type {expression.Type}");
            return new RemoteQueryable(elementType, this, expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            ExpressionHelper.CheckExpressionResultType<TResult>(expression);

            var rlinq = ExpressionHelper.TranslateExpression(expression, _context);
            var dataRecords = _dataProvider(rlinq);
            return _resultMapper.MapResult<TResult>(dataRecords, expression) !;
        }

        public object? Execute(Expression expression)
            => this.InvokeAndUnwrap<object?>(_executeMethod, expression);
    }
}
