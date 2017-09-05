// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class RemoteQueryable
    {
        /// <summary>
        /// Provides factory methods for creating <see cref="IQueryable{T}"/> 
        /// (or <see cref="IQueryable"/> respectively) suited for remote execution.
        /// </summary>
        public static RemoteQueryableFactory Factory { get; } = new RemoteQueryableFactory();

        /// <summary>
        /// Creates an instance of <see cref="IQueryable" /> that utilizes the data provider specified
        /// </summary>
        [Obsolete("Please use Remote.Linq.RemoteQueryable.Factory.Create() instead.", false)]
        public static IQueryable Create(Type elementType, Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider, ITypeResolver typeResolver = null, IDynamicObjectMapper mapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => Factory.Create(elementType, dataProvider, typeResolver, mapper, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IQueryable{T}" /> that utilizes the data provider specified
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IQueryable{T}"/></typeparam>
        [Obsolete("Please use Remote.Linq.RemoteQueryable.Factory.Create<T>() instead.", false)]
        public static IQueryable<T> Create<T>(Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider, ITypeResolver typeResolver = null, IDynamicObjectMapper mapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => Factory.Create<T>(dataProvider, typeResolver, mapper, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IQueryable" /> that utilizes the data provider specified
        /// </summary>
        [Obsolete("Please use Remote.Linq.RemoteQueryable.Factory.Create() instead.", false)]
        public static IQueryable Create(Type elementType, Func<Expressions.Expression, object> dataProvider, ITypeResolver typeResolver = null, IQueryResultMapper<object> resultMapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => Factory.Create(elementType, dataProvider, typeResolver, resultMapper, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IQueryable{T}" /> that utilizes the data provider specified
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IQueryable{T}"/></typeparam>
        [Obsolete("Please use Remote.Linq.RemoteQueryable.Factory.Create<T>() instead.", false)]
        public static IQueryable<T> Create<T>(Func<Expressions.Expression, object> dataProvider, ITypeResolver typeResolver = null, IQueryResultMapper<object> resultMapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => Factory.Create<T>(dataProvider, typeResolver, resultMapper, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IQueryable" /> that utilizes the data provider specified
        /// </summary>
        /// <typeparam name="TSource">Data type served by the data provider</typeparam>
        [Obsolete("Please use Remote.Linq.RemoteQueryable.Factory.Create<TSource>() instead.", false)]
        public static IQueryable Create<TSource>(Type elementType, Func<Expressions.Expression, TSource> dataProvider, IQueryResultMapper<TSource> resultMapper, ITypeResolver typeResolver = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => Factory.Create<TSource>(elementType, dataProvider, resultMapper, typeResolver, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IQueryable{T}" /> that utilizes the data provider specified
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IQueryable{T}"/></typeparam>
        /// <typeparam name="TSource">Data type served by the data provider</typeparam>
        [Obsolete("Please use Remote.Linq.RemoteQueryable.Factory.Create<T,TSource>() instead.", false)]
        public static IQueryable<T> Create<T, TSource>(Func<Expressions.Expression, TSource> dataProvider, IQueryResultMapper<TSource> resultMapper, ITypeResolver typeResolver = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => Factory.Create<T, TSource>(dataProvider, resultMapper, typeResolver, canBeEvaluatedLocally);
    }
}
