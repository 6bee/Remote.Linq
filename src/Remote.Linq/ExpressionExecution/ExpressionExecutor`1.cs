// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution
{
    using Aqua.TypeSystem;
    using Aqua.TypeSystem.Extensions;
    using Remote.Linq.Expressions;
    using Remote.Linq.ExpressionVisitors;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    [SuppressMessage("Minor Code Smell", "S4136:Method overloads should be grouped together", Justification = "Methods appear in logical order")]
    public abstract class ExpressionExecutor<TDataTranferObject> : IExpressionExecutionDecorator<TDataTranferObject>
    {
        private readonly Func<Type, IQueryable> _queryableProvider;
        private readonly ITypeResolver? _typeResolver;
        private readonly Func<System.Linq.Expressions.Expression, bool>? _canBeEvaluatedLocally;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionExecutor{TDataTranferObject}"/> class.
        /// </summary>
        protected ExpressionExecutor(Func<Type, IQueryable> queryableProvider, ITypeResolver? typeResolver = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
        {
            _queryableProvider = queryableProvider;
            _typeResolver = typeResolver;
            _canBeEvaluatedLocally = canBeEvaluatedLocally;
        }

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/> and mappes the result into dynamic objects.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <returns>The mapped result of the query execution.</returns>
        public TDataTranferObject Execute(Expression expression)
        {
            var preparedRemoteExpression = Prepare(expression);
            var linqExpression = Transform(preparedRemoteExpression);
            var preparedLinqExpression = Prepare(linqExpression);
            var queryResult = Execute(preparedLinqExpression);
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
        /// Executes the <see cref="System.Linq.Expressions.Expression"/> and returns the raw result.
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
        protected virtual object? Execute(System.Linq.Expressions.Expression expression)
        {
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
        /// Executes the <see cref="System.Linq.Expressions.Expression"/> and returns the raw result.
        /// </summary>
        /// <param name="expression">The <see cref="System.Linq.Expressions.Expression"/> to be executed.</param>
        /// <returns>Execution result of the <see cref="System.Linq.Expressions.Expression"/> specified.</returns>
        protected static object? ExecuteCore(System.Linq.Expressions.Expression expression)
        {
            var queryResult = expression.CompileAndInvokeExpression();
            if (queryResult is null)
            {
                return null;
            }

            var queryableType = queryResult.GetType();
            if (queryableType.Implements(typeof(IQueryable<>)))
            {
                // force query execution
                var elementType = TypeHelper.GetElementType(queryableType);
                queryResult = MethodInfos.Enumerable.ToArray.MakeGenericMethod(elementType).InvokeAndUnwrap(null, queryResult);
            }

            return queryResult;
        }

        /// <summary>
        /// If overriden in a derived class processes the raw query result.
        /// </summary>
        /// <param name="queryResult">The reult of the query execution.</param>
        /// <returns>Processed result.</returns>
        protected virtual object? ProcessResult(object? queryResult) => queryResult;

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
        protected virtual TDataTranferObject ProcessResult(TDataTranferObject queryResult) => queryResult;

        Expression IExpressionExecutionDecorator<TDataTranferObject>.Prepare(Expression expression) => Prepare(expression);

        System.Linq.Expressions.Expression IExpressionExecutionDecorator<TDataTranferObject>.Transform(Expression expression) => Transform(expression);

        System.Linq.Expressions.Expression IExpressionExecutionDecorator<TDataTranferObject>.Prepare(System.Linq.Expressions.Expression expression) => Prepare(expression);

        object? IExpressionExecutionDecorator<TDataTranferObject>.Execute(System.Linq.Expressions.Expression expression) => Execute(expression);

        object? IExpressionExecutionDecorator<TDataTranferObject>.ProcessResult(object? queryResult) => ProcessResult(queryResult);

        TDataTranferObject IExpressionExecutionDecorator<TDataTranferObject>.ConvertResult(object? queryResult) => ConvertResult(queryResult);

        TDataTranferObject IExpressionExecutionDecorator<TDataTranferObject>.ProcessResult(TDataTranferObject queryResult) => ProcessResult(queryResult);
    }
}
