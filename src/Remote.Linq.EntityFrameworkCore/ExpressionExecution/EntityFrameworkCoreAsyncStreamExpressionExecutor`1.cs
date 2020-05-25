// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#if ASYNC_STREAM

namespace Remote.Linq.EntityFrameworkCore.ExpressionExecution
{
    using Aqua.TypeSystem;
    using Aqua.TypeSystem.Extensions;
    using Microsoft.EntityFrameworkCore;
    using Remote.Linq.EntityFrameworkCore.ExpressionVisitors;
    using Remote.Linq.ExpressionExecution;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Security;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class EntityFrameworkCoreAsyncStreamExpressionExecutor<TDataTranferObject> : AsyncStreamExpressionExecutor<TDataTranferObject>
        where TDataTranferObject : class
    {
        private static readonly System.Reflection.MethodInfo ToListAsync = typeof(EntityFrameworkQueryableExtensions)
            .GetTypeInfo()
            .GetDeclaredMethods(nameof(EntityFrameworkQueryableExtensions.ToListAsync))
            .Where(m => m.IsGenericMethodDefinition && m.GetParameters().Length == 2)
            .Single();

        private static readonly Func<Type, System.Reflection.PropertyInfo> TaskResultProperty = (Type resultType) =>
            typeof(Task<>).MakeGenericType(resultType)
                .GetProperty(nameof(Task<object?>.Result));

        [SecuritySafeCritical]
        protected EntityFrameworkCoreAsyncStreamExpressionExecutor(DbContext dbContext, ITypeResolver? typeResolver = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            : this(dbContext.GetQueryableSetProvider(), typeResolver, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated))
        {
        }

        protected EntityFrameworkCoreAsyncStreamExpressionExecutor(Func<Type, IQueryable> queryableProvider, ITypeResolver? typeResolver = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            : base(queryableProvider, typeResolver, canBeEvaluatedLocally)
        {
        }

        protected override Expressions.Expression Prepare(Expressions.Expression expression)
            => base.Prepare(expression).ReplaceIncludeMethodCall();

        protected override IAsyncEnumerable<object?> ExecuteAsyncStream(System.Linq.Expressions.Expression expression)
        {
            if (!expression.Type.Implements(typeof(IQueryable<>)))
            {
                throw new ArgumentException("Expression must be of type IQueryable<>");
            }

            var queryable = (IQueryable)expression.CompileAndInvokeExpression() !;
            return queryable.ExecuteAsAsyncStream();
        }
    }
}

#endif // ASYNC_STREAM
