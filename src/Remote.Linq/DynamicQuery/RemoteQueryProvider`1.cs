// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Aqua.TypeSystem;
    using Remote.Linq.ExpressionVisitors;
    using System;
    using System.Linq;
    using Expression = System.Linq.Expressions.Expression;
    using MethodInfo = System.Reflection.MethodInfo;

    internal sealed class RemoteQueryProvider<TSource> : IRemoteQueryProvider
    {
        private static readonly MethodInfo _executeMethod = typeof(RemoteQueryProvider<TSource>)
            .GetMethods()
            .Single(x => x.IsGenericMethod && string.Equals(x.Name, nameof(Execute), StringComparison.Ordinal));

        private readonly Func<Expressions.Expression, TSource> _dataProvider;
        private readonly IQueryResultMapper<TSource> _resultMapper;
        private readonly ITypeInfoProvider _typeInfoProvider;
        private readonly Func<Expression, bool> _canBeEvaluatedLocally;

        internal RemoteQueryProvider(Func<Expressions.Expression, TSource> dataProvider, ITypeInfoProvider typeInfoProvider, IQueryResultMapper<TSource> resultMapper, Func<Expression, bool> canBeEvaluatedLocally)
        {
            _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
            _resultMapper = resultMapper ?? throw new ArgumentNullException(nameof(resultMapper));
            _typeInfoProvider = typeInfoProvider;
            _canBeEvaluatedLocally = canBeEvaluatedLocally;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            => new RemoteQueryable<TElement>(this, expression);

        public IQueryable CreateQuery(Expression expression)
        {
            var elementType = TypeHelper.GetElementType(expression.Type);
            return new RemoteQueryable(elementType, this, expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var rlinq = TranslateExpression(expression, _typeInfoProvider, _canBeEvaluatedLocally);
            var dataRecords = _dataProvider(rlinq);
            var result = Equals(default(TSource), dataRecords)
                ? default
                : _resultMapper.MapResult<TResult>(dataRecords, expression);
            return result;
        }

        public object Execute(Expression expression)
            => this.InvokeAndUnwrap<object>(_executeMethod, expression);

        internal static Expressions.Expression TranslateExpression(Expression expression, ITypeInfoProvider typeInfoProvider, Func<Expression, bool> canBeEvaluatedLocally)
        {
            var slinq1 = expression.SimplifyIncorporationOfRemoteQueryables();
            var rlinq1 = slinq1.ToRemoteLinqExpression(typeInfoProvider, canBeEvaluatedLocally);
            var rlinq2 = rlinq1.ReplaceQueryableByResourceDescriptors(typeInfoProvider);
            var rlinq3 = rlinq2.ReplaceGenericQueryArgumentsByNonGenericArguments();
            return rlinq3;
        }
    }
}
