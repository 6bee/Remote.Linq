// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.Dynamic;
    using Remote.Linq.ExpressionExecution;
    using Remote.Linq.Expressions;
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Security;

    internal class QueryService
    {
        private static readonly Func<Type, IQueryable> _queryableProvider = type =>
        {
            Array single = Array.CreateInstance(type, 1);
            object queryable = typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Single(x => string.Equals(x.Name, nameof(Queryable.AsQueryable)) && x.IsGenericMethodDefinition)
                .MakeGenericMethod(type)
                .Invoke(null, new object[] { single });
            return (IQueryable)queryable;
        };

        [SecuritySafeCritical]
        public DynamicObject ExecuteQuery(Expression queryExpression)
            => queryExpression.Execute(_queryableProvider);
    }
}