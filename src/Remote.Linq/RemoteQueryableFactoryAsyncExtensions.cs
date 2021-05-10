// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using Remote.Linq.DynamicQuery;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;
    using RemoteLinq = Remote.Linq.Expressions;
    using SystemLinq = System.Linq.Expressions;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class RemoteQueryableFactoryAsyncExtensions
    {
        private const string TaskBasedMethodObsolete = "This method can no longer be used and will be removed in a future version. Make sure to call `CreateAsyncQueryable` instead and provide ValueTask<> based data provider.";

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IRemoteQueryable CreateQueryable(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, Task<IEnumerable<DynamicObject>>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable<T> CreateQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, Task<IEnumerable<DynamicObject>>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IRemoteQueryable CreateQueryable(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, Task<object>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IAsyncQueryResultMapper<object>? resultMapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable<T> CreateQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, Task<object>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IAsyncQueryResultMapper<object>? resultMapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IRemoteQueryable CreateQueryable<TSource>(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, Task<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IRemoteQueryable CreateQueryable<TSource>(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, CancellationToken, Task<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable<T> CreateQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, Task<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable<T> CreateQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, CancellationToken, Task<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable CreateQueryable(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, ValueTask<DynamicObject>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable CreateQueryable(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, CancellationToken, ValueTask<DynamicObject>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable<T> CreateQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, ValueTask<DynamicObject>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable<T> CreateQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, CancellationToken, ValueTask<DynamicObject>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable CreateQueryable(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, ValueTask<object>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IAsyncQueryResultMapper<object>? resultMapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable CreateQueryable(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, CancellationToken, ValueTask<object>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IAsyncQueryResultMapper<object>? resultMapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable<T> CreateQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, ValueTask<object>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IAsyncQueryResultMapper<object>? resultMapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable<T> CreateQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, CancellationToken, ValueTask<object>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IAsyncQueryResultMapper<object>? resultMapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable CreateQueryable<TSource>(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, ValueTask<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable CreateQueryable<TSource>(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable<T> CreateQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, ValueTask<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable<T> CreateQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
        /// </summary>
        public static IAsyncRemoteQueryable CreateAsyncQueryable(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, ValueTask<DynamicObject>> dataProvider,
            IDynamicObjectMapper? mapper = null,
            IExpressionToRemoteLinqContext? context = null)
            => CreateAsyncQueryable<DynamicObject>(factory, elementType, dataProvider!, new AsyncDynamicResultMapper(mapper), context);

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
        /// </summary>
        public static IAsyncRemoteQueryable CreateAsyncQueryable(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, ValueTask<DynamicObject>> dataProvider,
            ITypeInfoProvider? typeInfoProvider,
            IDynamicObjectMapper? mapper = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateAsyncQueryable(factory, elementType, dataProvider, mapper, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
        /// </summary>
        public static IAsyncRemoteQueryable CreateAsyncQueryable(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, CancellationToken, ValueTask<DynamicObject>> dataProvider,
            IDynamicObjectMapper? mapper = null,
            IExpressionToRemoteLinqContext? context = null)
            => CreateAsyncQueryable<DynamicObject>(factory, elementType, dataProvider!, new AsyncDynamicResultMapper(mapper), context);

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
        /// </summary>
        public static IAsyncRemoteQueryable CreateAsyncQueryable(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, CancellationToken, ValueTask<DynamicObject>> dataProvider,
            ITypeInfoProvider? typeInfoProvider,
            IDynamicObjectMapper? mapper = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateAsyncQueryable(factory, elementType, dataProvider, mapper, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, ValueTask<DynamicObject>> dataProvider,
            IDynamicObjectMapper? mapper = null,
            IExpressionToRemoteLinqContext? context = null)
            => CreateAsyncQueryable<T, DynamicObject>(factory, dataProvider!, new AsyncDynamicResultMapper(mapper), context);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, ValueTask<DynamicObject>> dataProvider,
            ITypeInfoProvider? typeInfoProvider,
            IDynamicObjectMapper? mapper = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateAsyncQueryable<T>(factory, dataProvider, mapper, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, CancellationToken, ValueTask<DynamicObject>> dataProvider,
            IDynamicObjectMapper? mapper = null,
            IExpressionToRemoteLinqContext? context = null)
            => CreateAsyncQueryable<T, DynamicObject>(factory, dataProvider!, new AsyncDynamicResultMapper(mapper), context);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, CancellationToken, ValueTask<DynamicObject>> dataProvider,
            ITypeInfoProvider? typeInfoProvider,
            IDynamicObjectMapper? mapper = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateAsyncQueryable<T>(factory, dataProvider, mapper, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
        /// </summary>
        public static IAsyncRemoteQueryable CreateAsyncQueryable(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, ValueTask<object?>> dataProvider,
            IAsyncQueryResultMapper<object>? resultMapper = null,
            IExpressionToRemoteLinqContext? context = null)
            => CreateAsyncQueryable<object>(factory, elementType, dataProvider, resultMapper ?? new AsyncObjectResultCaster(), context);

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
        /// </summary>
        public static IAsyncRemoteQueryable CreateAsyncQueryable(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, ValueTask<object?>> dataProvider,
            ITypeInfoProvider? typeInfoProvider,
            IAsyncQueryResultMapper<object>? resultMapper = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateAsyncQueryable(factory, elementType, dataProvider, resultMapper, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
        /// </summary>
        public static IAsyncRemoteQueryable CreateAsyncQueryable(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, CancellationToken, ValueTask<object?>> dataProvider,
            IAsyncQueryResultMapper<object>? resultMapper = null,
            IExpressionToRemoteLinqContext? context = null)
            => CreateAsyncQueryable<object>(factory, elementType, dataProvider, resultMapper ?? new AsyncObjectResultCaster(), context);

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
        /// </summary>
        public static IAsyncRemoteQueryable CreateAsyncQueryable(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, CancellationToken, ValueTask<object?>> dataProvider,
            ITypeInfoProvider? typeInfoProvider,
            IAsyncQueryResultMapper<object>? resultMapper = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateAsyncQueryable(factory, elementType, dataProvider, resultMapper, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}"/> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, ValueTask<object?>> dataProvider,
            IAsyncQueryResultMapper<object>? resultMapper = null,
            IExpressionToRemoteLinqContext? context = null)
            => CreateAsyncQueryable<T, object>(factory, dataProvider, resultMapper ?? new AsyncObjectResultCaster(), context);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}"/> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, ValueTask<object?>> dataProvider,
            ITypeInfoProvider? typeInfoProvider,
            IAsyncQueryResultMapper<object>? resultMapper = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateAsyncQueryable<T>(factory, dataProvider, resultMapper, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}"/> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, CancellationToken, ValueTask<object?>> dataProvider,
            IAsyncQueryResultMapper<object>? resultMapper = null,
            IExpressionToRemoteLinqContext? context = null)
            => CreateAsyncQueryable<T, object>(factory, dataProvider, resultMapper ?? new AsyncObjectResultCaster(), context);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}"/> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, CancellationToken, ValueTask<object?>> dataProvider,
            ITypeInfoProvider? typeInfoProvider,
            IAsyncQueryResultMapper<object>? resultMapper = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateAsyncQueryable<T>(factory, dataProvider, resultMapper, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IAsyncRemoteQueryable CreateAsyncQueryable<TSource>(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, ValueTask<TSource?>> dataProvider,
            IAsyncQueryResultMapper<TSource> resultMapper,
            IExpressionToRemoteLinqContext? context = null)
        {
            dataProvider.AssertNotNull(nameof(dataProvider));
            return CreateAsyncQueryable<TSource>(factory, elementType, (expression, _) => dataProvider(expression), resultMapper, context);
        }

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IAsyncRemoteQueryable CreateAsyncQueryable<TSource>(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, ValueTask<TSource?>> dataProvider,
            IAsyncQueryResultMapper<TSource> resultMapper,
            ITypeInfoProvider? typeInfoProvider,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateAsyncQueryable<TSource>(factory, elementType, dataProvider, resultMapper, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IAsyncRemoteQueryable CreateAsyncQueryable<TSource>(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource?>> dataProvider,
            IAsyncQueryResultMapper<TSource> resultMapper,
            IExpressionToRemoteLinqContext? context = null)
        {
            var queryProvider = new AsyncRemoteQueryProvider<TSource>(dataProvider, resultMapper, context);
            return new AsyncRemoteQueryable(elementType, queryProvider);
        }

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IAsyncRemoteQueryable CreateAsyncQueryable<TSource>(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource?>> dataProvider,
            IAsyncQueryResultMapper<TSource> resultMapper,
            ITypeInfoProvider? typeInfoProvider,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateAsyncQueryable<TSource>(factory, elementType, dataProvider, resultMapper, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, ValueTask<TSource?>> dataProvider,
            IAsyncQueryResultMapper<TSource> resultMapper,
            IExpressionToRemoteLinqContext? context = null)
        {
            dataProvider.AssertNotNull(nameof(dataProvider));
            return factory.CreateAsyncQueryable<T, TSource>((expression, _) => dataProvider(expression), resultMapper, context);
        }

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, ValueTask<TSource?>> dataProvider,
            IAsyncQueryResultMapper<TSource> resultMapper,
            ITypeInfoProvider? typeInfoProvider,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateAsyncQueryable<T, TSource>(factory, dataProvider, resultMapper, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource?>> dataProvider,
            IAsyncQueryResultMapper<TSource> resultMapper,
            IExpressionToRemoteLinqContext? context = null)
        {
            var queryProvider = new AsyncRemoteQueryProvider<TSource>(dataProvider, resultMapper, context);
            return new AsyncRemoteQueryable<T>(queryProvider);
        }

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource?>> dataProvider,
            IAsyncQueryResultMapper<TSource> resultMapper,
            ITypeInfoProvider? typeInfoProvider,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateAsyncQueryable<T, TSource>(factory, dataProvider, resultMapper, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));
    }
}