// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using Remote.Linq.Expressions;
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class EntityFrameworkExpressionExecutor : ExpressionExecutor
    {
        private static readonly System.Reflection.MethodInfo DbContextSetMethod = typeof(DbContext).GetMethods()
                .Single(x => x.Name == "Set" && x.IsGenericMethod && x.GetGenericArguments().Length == 1 && x.GetParameters().Length == 0);

        public EntityFrameworkExpressionExecutor(DbContext dbContext, ITypeResolver typeResolver = null, IDynamicObjectMapper mapper = null, Func<Type, bool> setTypeInformation = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            : this(GetQueryableSetProvider(dbContext), typeResolver, mapper, setTypeInformation, canBeEvaluatedLocally)
        {
        }

        public EntityFrameworkExpressionExecutor(Func<Type, IQueryable> queryableProvider, ITypeResolver typeResolver = null, IDynamicObjectMapper mapper = null, Func<Type, bool> setTypeInformation = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            : base(queryableProvider, typeResolver, mapper, setTypeInformation, canBeEvaluatedLocally)
        {
        }

        /// <summary>
        /// Returns the generic <see cref="DbSet{T}"/> for the type specified
        /// </summary>
        /// <returns>Returns an instance of type <see cref="DbSet{T}"/></returns>
        private static Func<Type, IQueryable> GetQueryableSetProvider(DbContext dbContext) => (Type type) =>
        {
            var method = DbContextSetMethod.MakeGenericMethod(type);
            var set = method.Invoke(dbContext, new object[0]);
            return (IQueryable)set;
        };

        protected override System.Linq.Expressions.Expression PrepareForExecution(Expression expression)
        {
            var expression1 = expression.ReplaceIncludeMethodCall();
            var linqExpression1 = base.PrepareForExecution(expression1);
            var linqExpression2 = linqExpression1.ReplaceParameterizedConstructorCallsForVariableQueryArguments();
            return linqExpression2;
        }

        internal System.Linq.Expressions.Expression PrepareForExecutionWithEntityFramework(Expression expression)
            => PrepareForExecution(expression);
    }
}
