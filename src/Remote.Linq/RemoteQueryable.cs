// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Remote.Linq
{
    public class RemoteQueryable : IQueryable
    {
        protected readonly Type _elemntType;
        protected readonly Expression _expression;
        protected readonly IQueryProvider _provider;

        internal RemoteQueryable(Type elementType, Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider)
        {
            if (dataProvider == null) throw new ArgumentNullException("dataProvider");

            _elemntType = elementType;
            _expression = Expression.Constant(this);
            _provider = new RemoteQueryProvider(dataProvider);
        }

        internal RemoteQueryable(Type elemntType, IQueryProvider provider, Expression expression)
        {
            _elemntType = elemntType;
            _expression = expression;
            _provider = provider;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_provider.Execute<System.Collections.IEnumerable>(_expression)).GetEnumerator();
        }

        Type IQueryable.ElementType { get { return _elemntType; } }

        Expression IQueryable.Expression { get { return _expression; } }

        IQueryProvider IQueryable.Provider { get { return _provider; } }

        /// <summary>
        /// Creates an instance of <see cref="IQueryable" /> that utilizes the data provider specified
        /// </summary>
        public static IQueryable Create(Type elementType, Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider)
        {
            return new RemoteQueryable(elementType, dataProvider);
        }

        /// <summary>
        /// Creates an instance of <see cref="IQueryable{T}" /> that utilizes the data provider specified
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        public static IQueryable<T> Create<T>(Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider)
        {
            return new RemoteQueryable<T>(dataProvider);
        }
    }
}
