// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Remote.Linq.ExpressionVisitors;
    using Remote.Linq.TypeSystem;
    using System;
    using System.Linq;
    using Expression = System.Linq.Expressions.Expression;

    internal sealed partial class RemoteQueryProvider<TSource> : IRemoteQueryProvider
    {
        private readonly Func<Expressions.Expression, TSource> _dataProvider;
        private readonly IQueryResultMapper<TSource> _resultMapper;
        private readonly ITypeResolver _typeResolver;

        internal RemoteQueryProvider(Func<Expressions.Expression, TSource> dataProvider, ITypeResolver typeResolver, IQueryResultMapper<TSource> resultMapper)
        {
            if (ReferenceEquals(null, dataProvider))
            {
                throw new ArgumentNullException("dataProvider");
            }

            _dataProvider = dataProvider;
            _resultMapper = resultMapper;
            _typeResolver = typeResolver;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new RemoteQueryable<TElement>(this, expression);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            var elementType = TypeHelper.GetElementType(expression.Type);
            return new RemoteQueryable(elementType, this, expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var rlinq = TranslateExpression(expression, _typeResolver);
            var dataRecords = _dataProvider(rlinq);
            var result = _resultMapper.MapResult<TResult>(dataRecords);
            return result;
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        internal static Expressions.Expression TranslateExpression(Expression expression, ITypeResolver typeResolver)
        {
            var rlinq1 = expression.ToRemoteLinqExpression();
            var rlinq2 = rlinq1.ReplaceQueryableByResourceDescriptors(typeResolver);
            var rlinq3 = rlinq2.ReplaceGenericQueryArgumentsByNonGenericArguments();
            return rlinq3;
        }
    }
}
