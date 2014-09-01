// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.Dynamic;
using Remote.Linq.TypeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Remote.Linq
{
    internal sealed partial class RemoteQueryProvider : IQueryProvider
    {
        private readonly Func<Expressions.Expression, IEnumerable<DynamicObject>> _dataProvider;

        internal RemoteQueryProvider(Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider)
        {
            if (ReferenceEquals(null, dataProvider)) throw new ArgumentNullException("dataProvider");
            _dataProvider = dataProvider;
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
            var rlinq = TranslateExpression(expression);
            var dataRecords = _dataProvider(rlinq);
            return DynamicObjectMapper.ToType<TResult>(dataRecords);
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        internal static Expressions.Expression TranslateExpression(Expression expression)
        {
            var rlinq1 = expression.ToRemoteLinqExpression();
            var rlinq2 = rlinq1.ReplaceQueryableByResourceDescriptors();
            return rlinq2;
        }
    }
}
