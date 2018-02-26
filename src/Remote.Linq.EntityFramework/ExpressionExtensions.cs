// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

    public static class ExpressionExtensions
    {
        private static readonly System.Reflection.MethodInfo DbContextSetMethod = typeof(DbContext).GetMethods()
                .Single(x => x.Name == "Set" && x.IsGenericMethod && x.GetGenericArguments().Length == 1 && x.GetParameters().Length == 0);

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/> and mappes the result into dynamic objects
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed</param>
        /// <param name="dbContext">Instance of <see cref="DbContext"/> to get the <see cref="DbSet{T}"/></param>
        /// <param name="typeResolver">Optional instance of <see cref="ITypeResolver"/> to be used to translate <see cref="Aqua.TypeSystem.TypeInfo"/> into <see cref="Type"/> objects</param>
        /// <param name="mapper">Optional instance of <see cref="IDynamicObjectMapper"/></param>
        /// <returns>The mapped result of the query execution</returns>
        public static IEnumerable<DynamicObject> ExecuteWithEntityFramework(this Expression expression, DbContext dbContext, ITypeResolver typeResolver = null, IDynamicObjectMapper mapper = null, Func<Type, bool> setTypeInformation = null)
        {
            return ExecuteWithEntityFramework(expression, dbContext.GetQueryableSet, typeResolver, mapper, setTypeInformation);
        }

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/> and mappes the result into dynamic objects
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed</param>
        /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances based on <see cref="Type"/>s</param>
        /// <param name="typeResolver">Optional instance of <see cref="ITypeResolver"/> to be used to translate <see cref="Aqua.TypeSystem.TypeInfo"/> into <see cref="Type"/> objects</param>
        /// <param name="mapper">Optional instance of <see cref="IDynamicObjectMapper"/></param>
        /// <returns>The mapped result of the query execution</returns>
        public static IEnumerable<DynamicObject> ExecuteWithEntityFramework(this Expression expression, Func<Type, IQueryable> queryableProvider, ITypeResolver typeResolver = null, IDynamicObjectMapper mapper = null, Func<Type, bool> setTypeInformation = null)
        {
            var linqExpression = PrepareForExecutionWithEntityFramework(expression, queryableProvider, typeResolver);

            var queryResult = Remote.Linq.Expressions.ExpressionExtensions.Execute(linqExpression);

            var dynamicObjects = Remote.Linq.Expressions.ExpressionExtensions.ConvertResultToDynamicObjects(queryResult, mapper, setTypeInformation);
            return dynamicObjects;
        }

        /// <summary>
        /// Prepares the query <see cref="Expression"/> to be able to be executed. <para/> 
        /// Use this method if you wan to execute the <see cref="System.Linq.Expressions.Expression"/> and map the raw result yourself.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed</param>
        /// <param name="dbContext">Instance of <see cref="DbContext"/> to get the <see cref="DbSet{}"/></param>
        /// <param name="typeResolver">Optional instance of <see cref="ITypeResolver"/> to be used to translate <see cref="Aqua.TypeSystem.TypeInfo"/> into <see cref="Type"/> objects</param>
        /// <returns>A <see cref="System.Linq.Expressions.Expression"/> ready for execution</returns>
        public static System.Linq.Expressions.Expression PrepareForExecutionWithEntityFramework(this Expression expression, DbContext dbContext, ITypeResolver typeResolver = null)
        {
            return PrepareForExecutionWithEntityFramework(expression, dbContext.GetQueryableSet, typeResolver);
        }

        /// <summary>
        /// Prepares the query <see cref="Expression"/> to be able to be executed. <para/> 
        /// Use this method if you wan to execute the <see cref="System.Linq.Expressions.Expression"/> and map the raw result yourself. <para/> 
        /// Please note that Inlude operation has no effect if using non-generic method <see cref="IQueryable" /> <see cref="DbContext" />.Get(<see cref="Type" />) as queryableProvider.
        /// Better use generic version instead.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed</param>
        /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances based on <see cref="Type"/>s</param>
        /// <param name="typeResolver">Optional instance of <see cref="ITypeResolver"/> to be used to translate <see cref="Aqua.TypeSystem.TypeInfo"/> into <see cref="Type"/> objects</param>
        /// <returns>A <see cref="System.Linq.Expressions.Expression"/> ready for execution</returns>
        public static System.Linq.Expressions.Expression PrepareForExecutionWithEntityFramework(this Expression expression, Func<Type, IQueryable> queryableProvider, ITypeResolver typeResolver = null)
        {
            var expression1 = expression.ReplaceIncludeMethodCall();

            var linqExpression1 = expression1.PrepareForExecution(queryableProvider, typeResolver);

            var linqExpression2 = linqExpression1.ReplaceParameterizedConstructorCallsForVariableQueryArguments();

            return linqExpression2;
        }

        /// <summary>
        /// Returns the generic <see cref="DbSet{T}"/> for the type specified
        /// </summary>
        /// <returns>Returns an instance of type <see cref="DbSet{T}"/></returns>
        private static IQueryable GetQueryableSet(this DbContext dbContext, Type type)
        {
            var method = DbContextSetMethod.MakeGenericMethod(type);
            var set = method.Invoke(dbContext, new object[0]);
            return (IQueryable)set;
        }
    }
}
