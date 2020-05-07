// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.Entity;
    using System.Linq;
    using System.Security;
    using System.Threading;
    using System.Threading.Tasks;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Creates an <see cref="ExpressionExecutionContext" /> for the given <see cref="Expression"/>.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="dbContext">Instance of <see cref="DbContext"/> to get the <see cref="DbSet{T}"/>.</param>
        /// <param name="typeResolver">Optional instance of <see cref="ITypeResolver"/> to be used to translate <see cref="Aqua.TypeSystem.TypeInfo"/> into <see cref="Type"/> objects.</param>
        /// <param name="mapper">Optional instance of <see cref="IDynamicObjectMapper"/>.</param>
        /// <param name="setTypeInformation">Function to define whether to add type information.</param>
        /// <returns>A new instance <see cref="ExpressionExecutionContext" />.</returns>
        [SecuritySafeCritical]
        public static ExpressionExecutionContext EntityFrameworkExecutor(this Expression expression, DbContext dbContext, ITypeResolver typeResolver = null, IDynamicObjectMapper mapper = null, Func<Type, bool> setTypeInformation = null)
            => new ExpressionExecutionContext(new EntityFrameworkExpressionExecutor(dbContext, typeResolver, mapper, setTypeInformation), expression);

        /// <summary>
        /// Creates an <see cref="ExpressionExecutionContext" /> for the given <see cref="Expression"/>.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances for given <see cref="Type"/>s.</param>
        /// <param name="typeResolver">Optional instance of <see cref="ITypeResolver"/> to be used to translate <see cref="Aqua.TypeSystem.TypeInfo"/> into <see cref="Type"/> objects.</param>
        /// <param name="mapper">Optional instance of <see cref="IDynamicObjectMapper"/>.</param>
        /// <param name="setTypeInformation">Function to define whether to add type information.</param>
        /// <returns>A new instance <see cref="ExpressionExecutionContext" />.</returns>
        public static ExpressionExecutionContext EntityFrameworkExecutor(this Expression expression, Func<Type, IQueryable> queryableProvider, ITypeResolver typeResolver = null, IDynamicObjectMapper mapper = null, Func<Type, bool> setTypeInformation = null)
            => new ExpressionExecutionContext(new EntityFrameworkExpressionExecutor(queryableProvider, typeResolver, mapper, setTypeInformation), expression);

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/> and mappes the result into dynamic objects.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="dbContext">Instance of <see cref="DbContext"/> to get the <see cref="DbSet{T}"/>.</param>
        /// <param name="typeResolver">Optional instance of <see cref="ITypeResolver"/> to be used to translate <see cref="Aqua.TypeSystem.TypeInfo"/> into <see cref="Type"/> objects.</param>
        /// <param name="mapper">Optional instance of <see cref="IDynamicObjectMapper"/>.</param>
        /// <param name="setTypeInformation">Function to define whether to add type information.</param>
        /// <returns>The mapped result of the query execution.</returns>
        [SecuritySafeCritical]
        public static IEnumerable<DynamicObject> ExecuteWithEntityFramework(this Expression expression, DbContext dbContext, ITypeResolver typeResolver = null, IDynamicObjectMapper mapper = null, Func<Type, bool> setTypeInformation = null)
            => new EntityFrameworkExpressionExecutor(dbContext, typeResolver, mapper, setTypeInformation).Execute(expression);

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/> and mappes the result into dynamic objects.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="dbContext">Instance of <see cref="DbContext"/> to get the <see cref="DbSet{T}"/>.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <param name="typeResolver">Optional instance of <see cref="ITypeResolver"/> to be used to translate <see cref="Aqua.TypeSystem.TypeInfo"/> into <see cref="Type"/> objects.</param>
        /// <param name="mapper">Optional instance of <see cref="IDynamicObjectMapper"/>.</param>
        /// <param name="setTypeInformation">Function to define whether to add type information.</param>
        /// <returns>The mapped result of the query execution.</returns>
        [SecuritySafeCritical]
        public static Task<IEnumerable<DynamicObject>> ExecuteWithEntityFrameworkAsync(this Expression expression, DbContext dbContext, CancellationToken cancellationToken = default, ITypeResolver typeResolver = null, IDynamicObjectMapper mapper = null, Func<Type, bool> setTypeInformation = null)
            => new EntityFrameworkExpressionExecutor(dbContext, typeResolver, mapper, setTypeInformation).ExecuteAsync(expression, cancellationToken);

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/> and mappes the result into dynamic objects.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances for given <see cref="Type"/>s.</param>
        /// <param name="typeResolver">Optional instance of <see cref="ITypeResolver"/> to be used to translate <see cref="Aqua.TypeSystem.TypeInfo"/> into <see cref="Type"/> objects.</param>
        /// <param name="mapper">Optional instance of <see cref="IDynamicObjectMapper"/>.</param>
        /// <param name="setTypeInformation">Function to define whether to add type information.</param>
        /// <returns>The mapped result of the query execution.</returns>
        public static IEnumerable<DynamicObject> ExecuteWithEntityFramework(this Expression expression, Func<Type, IQueryable> queryableProvider, ITypeResolver typeResolver = null, IDynamicObjectMapper mapper = null, Func<Type, bool> setTypeInformation = null)
            => new EntityFrameworkExpressionExecutor(queryableProvider, typeResolver, mapper, setTypeInformation).Execute(expression);

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/> and mappes the result into dynamic objects.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances for given <see cref="Type"/>s.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <param name="typeResolver">Optional instance of <see cref="ITypeResolver"/> to be used to translate <see cref="Aqua.TypeSystem.TypeInfo"/> into <see cref="Type"/> objects.</param>
        /// <param name="mapper">Optional instance of <see cref="IDynamicObjectMapper"/>.</param>
        /// <param name="setTypeInformation">Function to define whether to add type information.</param>
        /// <returns>The mapped result of the query execution.</returns>
        public static Task<IEnumerable<DynamicObject>> ExecuteWithEntityFrameworkAsync(this Expression expression, Func<Type, IQueryable> queryableProvider, CancellationToken cancellationToken = default, ITypeResolver typeResolver = null, IDynamicObjectMapper mapper = null, Func<Type, bool> setTypeInformation = null)
            => new EntityFrameworkExpressionExecutor(queryableProvider, typeResolver, mapper, setTypeInformation).ExecuteAsync(expression, cancellationToken);
    }
}
