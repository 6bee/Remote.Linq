// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution
{
    using Remote.Linq.ExpressionVisitors;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using RemoteLinq = Remote.Linq.Expressions;
    using SystemLinq = System.Linq.Expressions;

    [SuppressMessage("Minor Code Smell", "S4136:Method overloads should be grouped together", Justification = "Methods appear in logical order")]
    public abstract class AsyncStreamExpressionExecutor<TQueryable, TDataTranferObject> : IAsyncStreamExpressionExecutionDecorator<TDataTranferObject>
    {
        private readonly Func<Type, TQueryable> _queryableProvider;
        private readonly IExpressionFromRemoteLinqContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncStreamExpressionExecutor{TQueryable, TDataTranferObject}"/> class.
        /// </summary>
        protected AsyncStreamExpressionExecutor(Func<Type, TQueryable> queryableProvider, IExpressionFromRemoteLinqContext? context = null)
        {
            _queryableProvider = queryableProvider.CheckNotNull(nameof(queryableProvider));
            _context = context ?? ExpressionTranslatorContext.Default;
        }

        ExecutionContext IAsyncStreamExpressionExecutionDecorator<TDataTranferObject>.Context => Context;

        protected ExecutionContext Context { get; } = new ExecutionContext();

        /// <summary>
        /// Composes and executes the query based on the <see cref="RemoteLinq.Expression"/>.
        /// </summary>
        /// <param name="expression">The <see cref="RemoteLinq.Expression"/> to be executed.</param>
        /// <param name="cancellation">A <see cref="CancellationToken" /> to observe while waiting for the async operation to complete.</param>
        /// <returns>The mapped result of the query execution.</returns>
        public IAsyncEnumerable<TDataTranferObject> ExecuteAsyncStream(RemoteLinq.Expression expression, CancellationToken cancellation = default)
        {
            var ctx = Context;

            var preparedRemoteExpression = Prepare(expression);
            ctx.RemoteExpression = preparedRemoteExpression;

            var linqExpression = Transform(preparedRemoteExpression);

            var preparedLinqExpression = Prepare(linqExpression);
            ctx.SystemExpression = preparedLinqExpression;

            var queryResult = ExecuteAsyncStream(preparedLinqExpression, cancellation);

            var processedResult = ProcessResult(queryResult);

            var dataTransferObjects = ConvertResult(processedResult);

            var processedDataTransferObjects = ProcessResult(dataTransferObjects);
            return processedDataTransferObjects;
        }

        /// <summary>
        /// Prepares the <see cref="RemoteLinq.Expression"/> befor being transformed.<para/>
        /// </summary>
        /// <param name="expression">The <see cref="RemoteLinq.Expression"/>.</param>
        /// <returns>A <see cref="SystemLinq.Expression"/>.</returns>
        protected virtual RemoteLinq.Expression Prepare(RemoteLinq.Expression expression)
            => expression
            .ReplaceNonGenericQueryArgumentsByGenericArguments()
            .ReplaceResourceDescriptorsByQueryable(_queryableProvider, _context.TypeResolver);

        /// <summary>
        /// Transforms the <see cref="RemoteLinq.Expression"/> to a <see cref="SystemLinq.Expression"/>.
        /// </summary>
        /// <param name="expression">The <see cref="RemoteLinq.Expression"/> to be transformed.</param>
        /// <returns>A <see cref="SystemLinq.Expression"/>.</returns>
        protected virtual SystemLinq.Expression Transform(RemoteLinq.Expression expression)
            => expression.ToLinqExpression(_context);

        /// <summary>
        /// Prepares the query <see cref="SystemLinq.Expression"/> to be able to be executed.
        /// </summary>
        /// <param name="expression">The <see cref="SystemLinq.Expression"/> returned by the Transform method.</param>
        /// <returns>A <see cref="SystemLinq.Expression"/> ready for execution.</returns>
        protected virtual SystemLinq.Expression Prepare(SystemLinq.Expression expression)
            => expression.PartialEval(_context.CanBeEvaluatedLocally);

        /// <summary>
        /// Executes the <see cref="SystemLinq.Expression"/> and returns the raw async stream result.
        /// </summary>
        /// <remarks>
        /// <see cref="InvalidOperationException"/> get handled for failing
        /// <see cref="Queryable.Single{TSource}(IQueryable{TSource})"/> and
        /// <see cref="Queryable.Single{TSource}(IQueryable{TSource}, SystemLinq.Expression{Func{TSource, bool}})"/>,
        /// <see cref="Queryable.First{TSource}(IQueryable{TSource})"/>,
        /// <see cref="Queryable.First{TSource}(IQueryable{TSource}, SystemLinq.Expression{Func{TSource, bool}})"/>,
        /// <see cref="Queryable.Last{TSource}(IQueryable{TSource})"/>,
        /// <see cref="Queryable.Last{TSource}(IQueryable{TSource}, SystemLinq.Expression{Func{TSource, bool}})"/>.
        /// Instead of throwing an exception, an array with the length of zero respectively two elements is returned.
        /// </remarks>
        /// <param name="expression">The <see cref="SystemLinq.Expression"/> to be executed.</param>
        /// <param name="cancellation">A <see cref="CancellationToken" /> to observe while waiting for the async operation to complete.</param>
        /// <returns>Execution result of the <see cref="SystemLinq.Expression"/> specified.</returns>
        protected abstract IAsyncEnumerable<object?> ExecuteAsyncStream(SystemLinq.Expression expression, CancellationToken cancellation);

        /// <summary>
        /// If overriden in a derived class processes the items of the async stream.
        /// </summary>
        /// <param name="queryResult">The reult of the query execution.</param>
        /// <returns>Processed result.</returns>
        protected virtual IAsyncEnumerable<object?> ProcessResult(IAsyncEnumerable<object?> queryResult) => queryResult;

        /// <summary>
        /// Converts the async stream items into <typeparamref name="TDataTranferObject"/>.
        /// </summary>
        /// <param name="queryResult">The reult of the query execution.</param>
        /// <returns>The mapped query result.</returns>
        protected abstract IAsyncEnumerable<TDataTranferObject> ConvertResult(IAsyncEnumerable<object?> queryResult);

        /// <summary>
        /// If overriden in a derived class processes the <typeparamref name="TDataTranferObject"/> items of the async stream.
        /// </summary>
        /// <param name="queryResult">The reult of the query execution.</param>
        /// <returns>Processed result.</returns>
        protected virtual IAsyncEnumerable<TDataTranferObject> ProcessResult(IAsyncEnumerable<TDataTranferObject> queryResult)
            => queryResult;

        RemoteLinq.Expression IAsyncStreamExpressionExecutionDecorator<TDataTranferObject>.Prepare(RemoteLinq.Expression expression)
            => Prepare(expression);

        SystemLinq.Expression IAsyncStreamExpressionExecutionDecorator<TDataTranferObject>.Transform(RemoteLinq.Expression expression)
            => Transform(expression);

        SystemLinq.Expression IAsyncStreamExpressionExecutionDecorator<TDataTranferObject>.Prepare(SystemLinq.Expression expression)
            => Prepare(expression);

        IAsyncEnumerable<object?> IAsyncStreamExpressionExecutionDecorator<TDataTranferObject>.ExecuteAsyncStream(SystemLinq.Expression expression, CancellationToken cancellation)
            => ExecuteAsyncStream(expression, cancellation);

        IAsyncEnumerable<object?> IAsyncStreamExpressionExecutionDecorator<TDataTranferObject>.ProcessResult(IAsyncEnumerable<object?> queryResult)
            => ProcessResult(queryResult);

        IAsyncEnumerable<TDataTranferObject> IAsyncStreamExpressionExecutionDecorator<TDataTranferObject>.ConvertResult(IAsyncEnumerable<object?> queryResult)
            => ConvertResult(queryResult);

        IAsyncEnumerable<TDataTranferObject> IAsyncStreamExpressionExecutionDecorator<TDataTranferObject>.ProcessResult(IAsyncEnumerable<TDataTranferObject> queryResult)
            => ProcessResult(queryResult);
    }
}