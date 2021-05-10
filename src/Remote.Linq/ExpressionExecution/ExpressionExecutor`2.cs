// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution
{
    using Aqua.TypeExtensions;
    using Remote.Linq.ExpressionVisitors;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using RemoteLinq = Remote.Linq.Expressions;
    using SystemLinq = System.Linq.Expressions;

    [SuppressMessage("Minor Code Smell", "S4136:Method overloads should be grouped together", Justification = "Methods appear in logical order")]
    public abstract class ExpressionExecutor<TQueryable, TDataTranferObject> : IExpressionExecutionDecorator<TDataTranferObject>
    {
        private readonly Func<Type, TQueryable> _queryableProvider;
        private readonly IExpressionFromRemoteLinqContext? _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionExecutor{TQueryable, TDataTranferObject}"/> class.
        /// </summary>
        protected ExpressionExecutor(Func<Type, TQueryable> queryableProvider, IExpressionFromRemoteLinqContext? context = null)
        {
            _queryableProvider = queryableProvider;
            _context = context;
        }

        ExecutionContext IExpressionExecutionDecorator<TDataTranferObject>.Context => Context;

        protected ExecutionContext Context { get; } = new ExecutionContext();

        /// <summary>
        /// Composes and executes the query based on the <see cref="RemoteLinq.Expression"/> and mappes the result into dynamic objects.
        /// </summary>
        /// <param name="expression">The <see cref="RemoteLinq.Expression"/> to be executed.</param>
        /// <returns>The mapped result of the query execution.</returns>
        public TDataTranferObject Execute(RemoteLinq.Expression expression)
        {
            var ctx = Context;

            var preparedRemoteExpression = Prepare(expression);
            ctx.RemoteExpression = preparedRemoteExpression;

            var linqExpression = Transform(preparedRemoteExpression);

            var preparedLinqExpression = Prepare(linqExpression);
            ctx.SystemExpression = preparedLinqExpression;

            var queryResult = Execute(preparedLinqExpression);

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
            .ReplaceResourceDescriptorsByQueryable(_queryableProvider, _context?.TypeResolver);

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
            => expression.PartialEval(_context?.CanBeEvaluatedLocally);

        /// <summary>
        /// Executes the <see cref="SystemLinq.Expression"/> and returns the raw result.
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
        /// <returns>Execution result of the <see cref="SystemLinq.Expression"/> specified.</returns>
        protected virtual object? Execute(SystemLinq.Expression expression)
        {
            expression.CheckNotNull(nameof(expression));
            try
            {
                return ExecuteCore(expression);
            }
            catch (InvalidOperationException ex)
            {
                if (string.Equals(ex.Message, "Sequence contains no elements", StringComparison.Ordinal))
                {
                    return Array.CreateInstance(expression.Type, 0);
                }

                if (string.Equals(ex.Message, "Sequence contains no matching element", StringComparison.Ordinal))
                {
                    return Array.CreateInstance(expression.Type, 0);
                }

                if (string.Equals(ex.Message, "Sequence contains more than one element", StringComparison.Ordinal))
                {
                    return Array.CreateInstance(expression.Type, 2);
                }

                if (string.Equals(ex.Message, "Sequence contains more than one matching element", StringComparison.Ordinal))
                {
                    return Array.CreateInstance(expression.Type, 2);
                }

                throw;
            }
        }

        /// <summary>
        /// Executes the <see cref="SystemLinq.Expression"/> and returns the raw result.
        /// </summary>
        /// <param name="expression">The <see cref="SystemLinq.Expression"/> to be executed.</param>
        /// <returns>Execution result of the <see cref="SystemLinq.Expression"/> specified.</returns>
        protected static object? ExecuteCore(SystemLinq.Expression expression)
        {
            var queryResult = expression.CheckNotNull(nameof(expression)).CompileAndInvokeExpression();
            if (queryResult is null)
            {
                return null;
            }

            var queryableType = queryResult.GetType();
            if (queryableType.Implements(typeof(IQueryable<>), out var genericType))
            {
                // force query execution
                queryResult = MethodInfos.Enumerable.ToArray.MakeGenericMethod(genericType[0]).InvokeAndUnwrap(null, queryResult);
            }

            return queryResult;
        }

        /// <summary>
        /// If overriden in a derived class processes the raw query result.
        /// </summary>
        /// <param name="queryResult">The reult of the query execution.</param>
        /// <returns>Processed result.</returns>
        protected virtual object? ProcessResult(object? queryResult)
            => queryResult;

        /// <summary>
        /// Converts the raw query result into <typeparamref name="TDataTranferObject"/>.
        /// </summary>
        /// <param name="queryResult">The reult of the query execution.</param>
        /// <returns>The mapped query result.</returns>
        protected abstract TDataTranferObject ConvertResult(object? queryResult);

        /// <summary>
        /// If overriden in a derived processes the <typeparamref name="TDataTranferObject"/>.
        /// </summary>
        /// <param name="queryResult">The reult of the query execution.</param>
        /// <returns>Processed result.</returns>
        protected virtual TDataTranferObject ProcessResult(TDataTranferObject queryResult)
            => queryResult;

        RemoteLinq.Expression IExpressionExecutionDecorator<TDataTranferObject>.Prepare(RemoteLinq.Expression expression)
            => Prepare(expression);

        SystemLinq.Expression IExpressionExecutionDecorator<TDataTranferObject>.Transform(RemoteLinq.Expression expression)
            => Transform(expression);

        SystemLinq.Expression IExpressionExecutionDecorator<TDataTranferObject>.Prepare(SystemLinq.Expression expression)
            => Prepare(expression);

        object? IExpressionExecutionDecorator<TDataTranferObject>.Execute(SystemLinq.Expression expression)
            => Execute(expression);

        object? IExpressionExecutionDecorator<TDataTranferObject>.ProcessResult(object? queryResult)
            => ProcessResult(queryResult);

        TDataTranferObject IExpressionExecutionDecorator<TDataTranferObject>.ConvertResult(object? queryResult)
            => ConvertResult(queryResult);

        TDataTranferObject IExpressionExecutionDecorator<TDataTranferObject>.ProcessResult(TDataTranferObject queryResult)
            => ProcessResult(queryResult);
    }
}