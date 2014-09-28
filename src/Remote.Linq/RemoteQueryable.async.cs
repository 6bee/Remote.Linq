// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remote.Linq
{
    partial class RemoteQueryable 
    {
        protected RemoteQueryable(Type elementType, Func<Expressions.Expression, Task<IEnumerable<DynamicObject>>> dataProvider, Func<IDynamicObjectMapper> mapper = null)
            : this(elementType, new AsyncRemoteQueryProvider(dataProvider, mapper))
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="IQueryable" /> that utilizes the data provider specified
        /// </summary>
        public static IQueryable Create(Type elementType, Func<Expressions.Expression, Task<IEnumerable<DynamicObject>>> dataProvider, Func<IDynamicObjectMapper> mapper = null)
        {
            return new RemoteQueryable(elementType, dataProvider, mapper);
        }

        /// <summary>
        /// Creates an instance of <see cref="IQueryable{T}" /> that utilizes the data provider specified
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        public static IQueryable<T> Create<T>(Func<Expressions.Expression, Task<IEnumerable<DynamicObject>>> dataProvider, Func<IDynamicObjectMapper> mapper = null)
        {
            return new AsyncRemoteQueryable<T>(dataProvider, mapper);
        }
    }
}
