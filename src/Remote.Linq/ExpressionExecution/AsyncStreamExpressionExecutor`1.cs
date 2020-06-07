// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution
{
    using Aqua.TypeSystem;
    using Remote.Linq.Expressions;
    using Remote.Linq.ExpressionVisitors;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    [SuppressMessage("Minor Code Smell", "S4136:Method overloads should be grouped together", Justification = "Methods appear in logical order")]
    public abstract class AsyncStreamExpressionExecutor<TDataTranferObject> : IAsyncStreamExpressionExecutionDecorator<TDataTranferObject>
        where TDataTranferObject : class
    {
        private readonly Func<Type, IQueryable> _queryableProvider;
        private readonly ITypeResolver? _typeResolver;
        private readonly Func<System.Linq.Expressions.Expression, bool>? _canBeEvaluatedLocally;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncStreamExpressionExecutor{TDataTranferObject}"/> class.
        /// </summary>
        protected AsyncStreamExpressionExecutor(Func<Type, IQueryable> queryableProvider, ITypeResolver? typeResolver = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
        {
            _queryableProvider = queryableProvider;
            _typeResolver = typeResolver;
            _canBeEvaluatedLocally = canBeEvaluatedLocally;
        }

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/>.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <returns>The mapped result of the query execution.</returns>
        public IAsyncEnumerable<TDataTranferObject?> ExecuteAsyncStream(Expression expression)
        {
            var preparedRemoteExpression = Prepare(expression);
            var linqExpression = Transform(preparedRemoteExpression);
            var preparedLinqExpression = Prepare(linqExpression);
            var queryResult = ExecuteAsyncStream(preparedLinqExpression);
            var processedResult = ProcessResult(queryResult);
            var dataTransferObjects = ConvertResult(processedResult);
            var processedDataTransferObjects = ProcessResult(dataTransferObjects);
            return processedDataTransferObjects;
        }

        /// <summary>
        /// Prepares the <see cref="Expression"/> befor being transformed.<para/>
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/>.</param>
        /// <returns>A <see cref="System.Linq.Expressions.Expression"/>.</returns>
        protected virtual Expression Prepare(Expression expression)
            => expression
            .ReplaceNonGenericQueryArgumentsByGenericArguments()
            .ReplaceResourceDescriptorsByQueryable(_queryableProvider, _typeResolver);

        /// <summary>
        /// Transforms the <see cref="Expression"/> to a <see cref="System.Linq.Expressions.Expression"/>.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be transformed.</param>
        /// <returns>A <see cref="System.Linq.Expressions.Expression"/>.</returns>
        protected virtual System.Linq.Expressions.Expression Transform(Expression expression)
            => expression.ToLinqExpression(_typeResolver);

        /// <summary>
        /// Prepares the query <see cref="System.Linq.Expressions.Expression"/> to be able to be executed.
        /// </summary>
        /// <param name="expression">The <see cref="System.Linq.Expressions.Expression"/> returned by the Transform method.</param>
        /// <returns>A <see cref="System.Linq.Expressions.Expression"/> ready for execution.</returns>
        protected virtual System.Linq.Expressions.Expression Prepare(System.Linq.Expressions.Expression expression)
            => expression.PartialEval(_canBeEvaluatedLocally);

        /// <summary>
        /// Executes the <see cref="System.Linq.Expressions.Expression"/> and returns the raw async stream result.
        /// </summary>
        /// <remarks>
        /// <see cref="InvalidOperationException"/> get handled for failing
        /// <see cref="Queryable.Single{TSource}(IQueryable{TSource})"/> and
        /// <see cref="Queryable.Single{TSource}(IQueryable{TSource}, System.Linq.Expressions.Expression{Func{TSource, bool}})"/>,
        /// <see cref="Queryable.First{TSource}(IQueryable{TSource})"/>,
        /// <see cref="Queryable.First{TSource}(IQueryable{TSource}, System.Linq.Expressions.Expression{Func{TSource, bool}})"/>,
        /// <see cref="Queryable.Last{TSource}(IQueryable{TSource})"/>,
        /// <see cref="Queryable.Last{TSource}(IQueryable{TSource}, System.Linq.Expressions.Expression{Func{TSource, bool}})"/>.
        /// Instead of throwing an exception, an array with the length of zero respectively two elements is returned.
        /// </remarks>
        /// <param name="expression">The <see cref="System.Linq.Expressions.Expression"/> to be executed.</param>
        /// <returns>Execution result of the <see cref="System.Linq.Expressions.Expression"/> specified.</returns>
        protected abstract IAsyncEnumerable<object?> ExecuteAsyncStream(System.Linq.Expressions.Expression expression);

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
        protected abstract IAsyncEnumerable<TDataTranferObject?> ConvertResult(IAsyncEnumerable<object?> queryResult);

        /// <summary>
        /// If overriden in a derived class processes the <typeparamref name="TDataTranferObject"/> items of the async stream.
        /// </summary>
        /// <param name="queryResult">The reult of the query execution.</param>
        /// <returns>Processed result.</returns>
        protected virtual IAsyncEnumerable<TDataTranferObject?> ProcessResult(IAsyncEnumerable<TDataTranferObject?> queryResult) => queryResult;

        Expression IAsyncStreamExpressionExecutionDecorator<TDataTranferObject>.Prepare(Expression expression) => Prepare(expression);

        System.Linq.Expressions.Expression IAsyncStreamExpressionExecutionDecorator<TDataTranferObject>.Transform(Expression expression) => Transform(expression);

        System.Linq.Expressions.Expression IAsyncStreamExpressionExecutionDecorator<TDataTranferObject>.Prepare(System.Linq.Expressions.Expression expression) => Prepare(expression);

        IAsyncEnumerable<object?> IAsyncStreamExpressionExecutionDecorator<TDataTranferObject>.ExecuteAsyncStream(System.Linq.Expressions.Expression expression) => ExecuteAsyncStream(expression);

        IAsyncEnumerable<object?> IAsyncStreamExpressionExecutionDecorator<TDataTranferObject>.ProcessResult(IAsyncEnumerable<object?> queryResult) => ProcessResult(queryResult);

        IAsyncEnumerable<TDataTranferObject?> IAsyncStreamExpressionExecutionDecorator<TDataTranferObject>.ConvertResult(IAsyncEnumerable<object?> queryResult) => ConvertResult(queryResult);

        IAsyncEnumerable<TDataTranferObject?> IAsyncStreamExpressionExecutionDecorator<TDataTranferObject>.ProcessResult(IAsyncEnumerable<TDataTranferObject?> queryResult) => ProcessResult(queryResult);
    }
}
