// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using Microsoft.EntityFrameworkCore;
    using Remote.Linq.Expressions;
    using System;
    using System.Linq;
    using System.Reflection;

    public class EntityFrameworkCoreExpressionExecutor : ExpressionExecutor
    {
        private static readonly System.Reflection.MethodInfo DbContextSetMethod = typeof(DbContext).GetTypeInfo()
            .GetDeclaredMethods("Set")
            .Single(x => x.IsGenericMethod && x.GetGenericArguments().Length == 1 && x.GetParameters().Length == 0);

        public EntityFrameworkCoreExpressionExecutor(DbContext dbContext, ITypeResolver typeResolver = null, IDynamicObjectMapper mapper = null, Func<Type, bool> setTypeInformation = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            : this(GetQueryableSetProvider(dbContext), typeResolver, mapper, setTypeInformation, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated))
        {
        }

        public EntityFrameworkCoreExpressionExecutor(Func<Type, IQueryable> queryableProvider, ITypeResolver typeResolver = null, IDynamicObjectMapper mapper = null, Func<Type, bool> setTypeInformation = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            : base(queryableProvider, typeResolver, mapper, setTypeInformation, canBeEvaluatedLocally)
        {
        }

        protected override System.Linq.Expressions.Expression PrepareForExecution(Expression expression)
        {
            var expression1 = expression.ReplaceIncludeMethodCall();
            var linqExpression = base.PrepareForExecution(expression1);
            return linqExpression;
        }

        internal System.Linq.Expressions.Expression PrepareForExecutionWithEntityFrameworkCore(Expression expression)
            => PrepareForExecution(expression);

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
    }
}
