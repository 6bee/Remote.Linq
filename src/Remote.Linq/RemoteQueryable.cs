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
        /// Gets a factory for creating <see cref="IQueryable{T}"/>
        /// (or <see cref="IQueryable"/> respectively) suited for remote execution.
        /// </summary>
        public static RemoteQueryableFactory Factory { get; } = new RemoteQueryableFactory();

        /// <summary>
        /// Creates an instance of <see cref="IQueryable" /> that utilizes the data provider specified.
        /// </summary>
        [Obsolete("This method will be removed in future versions. Use Remote.Linq.RemoteQueryable.Factory.CreateQueryable() instead.", false)]
        public static IQueryable Create(Type elementType, Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider, ITypeInfoProvider typeInfoProvider = null, IDynamicObjectMapper mapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => Factory.CreateQueryable(elementType, dataProvider, typeInfoProvider, mapper, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IQueryable{T}"/>.</typeparam>
        [Obsolete("This method will be removed in future versions. Use Remote.Linq.RemoteQueryable.Factory.CreateQueryable<T>() instead.", false)]
        public static IQueryable<T> Create<T>(Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider, ITypeInfoProvider typeInfoProvider = null, IDynamicObjectMapper mapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => Factory.CreateQueryable<T>(dataProvider, typeInfoProvider, mapper, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IQueryable" /> that utilizes the data provider specified.
        /// </summary>
        [Obsolete("This method will be removed in future versions. Use Remote.Linq.RemoteQueryable.Factory.CreateQueryable() instead.", false)]
        public static IQueryable Create(Type elementType, Func<Expressions.Expression, object> dataProvider, ITypeInfoProvider typeInfoProvider = null, IQueryResultMapper<object> resultMapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => Factory.CreateQueryable(elementType, dataProvider, typeInfoProvider, resultMapper, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IQueryable{T}"/>.</typeparam>
        [Obsolete("This method will be removed in future versions. Use Remote.Linq.RemoteQueryable.Factory.CreateQueryable<T>() instead.", false)]
        public static IQueryable<T> Create<T>(Func<Expressions.Expression, object> dataProvider, ITypeInfoProvider typeInfoProvider = null, IQueryResultMapper<object> resultMapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => Factory.CreateQueryable<T>(dataProvider, typeInfoProvider, resultMapper, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IQueryable" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        [Obsolete("This method will be removed in future versions. Use Remote.Linq.RemoteQueryable.Factory.CreateQueryable<TSource>() instead.", false)]
        public static IQueryable Create<TSource>(Type elementType, Func<Expressions.Expression, TSource> dataProvider, IQueryResultMapper<TSource> resultMapper, ITypeInfoProvider typeInfoProvider = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => Factory.CreateQueryable<TSource>(elementType, dataProvider, resultMapper, typeInfoProvider, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IQueryable{T}"/>.</typeparam>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        [Obsolete("This method will be removed in future versions. Use Remote.Linq.RemoteQueryable.Factory.CreateQueryable<T,TSource>() instead.", false)]
        public static IQueryable<T> Create<T, TSource>(Func<Expressions.Expression, TSource> dataProvider, IQueryResultMapper<TSource> resultMapper, ITypeInfoProvider typeInfoProvider = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => Factory.CreateQueryable<T, TSource>(dataProvider, resultMapper, typeInfoProvider, canBeEvaluatedLocally);
    }
}
