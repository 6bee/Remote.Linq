// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework
{
    using Remote.Linq.Dynamic;
    using Remote.Linq.Expressions;
    using Remote.Linq.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.Entity;
    using System.Linq;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ExpressionExtensions
    {
        private static readonly System.Reflection.MethodInfo DbContextSetMethod = typeof(DbContext).GetMethods()
                .Single(x => x.Name == "Set" && x.IsGenericMethod && x.GetGenericArguments().Length == 1 && x.GetParameters().Length == 0);

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/> and mappes the result into dynamic objects
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed</param>
        /// <param name="dbContext">Instance of <see cref="DbContext"/> to get the <see cref="DbSet{}"/></param>
        /// <param name="typeResolver">Optional instance of <see cref="ITypeResolver"/> to be used to translate <see cref="Remote.Linq.TypeSystem.TypeInfo"/> into <see cref="Type"/> objects</param>
        /// <param name="mapper">Optional instance of <see cref="IDynamicObjectMapper"/></param>
        /// <returns>The mapped result of the query execution</returns>
        public static IEnumerable<DynamicObject> ExecuteWithEntityFramework(this Expression expression, DbContext dbContext, ITypeResolver typeResolver = null, IDynamicObjectMapper mapper = null)
        {
            return ExecuteWithEntityFramework(expression, dbContext.GetQueryableSet, typeResolver, mapper);
        }

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/> and mappes the result into dynamic objects
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed</param>
        /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances based on <see cref="Type"/>s</param>
        /// <param name="typeResolver">Optional instance of <see cref="ITypeResolver"/> to be used to translate <see cref="Remote.Linq.TypeSystem.TypeInfo"/> into <see cref="Type"/> objects</param>
        /// <param name="mapper">Optional instance of <see cref="IDynamicObjectMapper"/></param>
        /// <returns>The mapped result of the query execution</returns>
        public static IEnumerable<DynamicObject> ExecuteWithEntityFramework(this Expression expression, Func<Type, IQueryable> queryableProvider, ITypeResolver typeResolver = null, IDynamicObjectMapper mapper = null)
        {
            var linqExpression = PrepareForExecutionWithEntityFramework(expression, queryableProvider, typeResolver);

            var queryResult = Remote.Linq.Expressions.ExpressionExtensions.Execute(linqExpression);

            var dynamicObjects = Remote.Linq.Expressions.ExpressionExtensions.ConvertResultToDynamicObjects(queryResult, mapper);
            return dynamicObjects;
        }

        /// <summary>
        /// Prepares the query <see cref="Expression"/> to be able to be executed. <para/> 
        /// Use this method if you wan to execute the <see cref="System.Linq.Expressions.Expression"/> and map the raw result yourself.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed</param>
        /// <param name="dbContext">Instance of <see cref="DbContext"/> to get the <see cref="DbSet{}"/></param>
        /// <param name="typeResolver">Optional instance of <see cref="ITypeResolver"/> to be used to translate <see cref="Remote.Linq.TypeSystem.TypeInfo"/> into <see cref="Type"/> objects</param>
        /// <returns>A <see cref="System.Linq.Expressions.Expression"/> ready for execution</returns>
        public static System.Linq.Expressions.Expression PrepareForExecutionWithEntityFramework(this Expression expression, DbContext dbContext, ITypeResolver typeResolver = null)
        {
            return PrepareForExecutionWithEntityFramework(expression, dbContext.GetQueryableSet, typeResolver);
        }

        /// <summary>
        /// Prepares the query <see cref="Expression"/> to be able to be executed. <para/> 
        /// Use this method if you wan to execute the <see cref="System.Linq.Expressions.Expression"/> and map the raw result yourself.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed</param>
        /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances based on <see cref="Type"/>s</param>
        /// <param name="typeResolver">Optional instance of <see cref="ITypeResolver"/> to be used to translate <see cref="Remote.Linq.TypeSystem.TypeInfo"/> into <see cref="Type"/> objects</param>
        /// <returns>A <see cref="System.Linq.Expressions.Expression"/> ready for execution</returns>
        public static System.Linq.Expressions.Expression PrepareForExecutionWithEntityFramework(this Expression expression, Func<Type, IQueryable> queryableProvider, ITypeResolver typeResolver = null)
        {
            var queryableExpression = expression.ReplaceIncludeMethodCall(queryableProvider, typeResolver);

            var linqExpression = queryableExpression.ToLinqExpression();
            return linqExpression;
        }

        private static IQueryable GetQueryableSet(this DbContext dbContext, Type type)
        {
            var method = DbContextSetMethod.MakeGenericMethod(type);
            var set = method.Invoke(dbContext, new object[0]);
            return (IQueryable)set;
        }
    }
}
