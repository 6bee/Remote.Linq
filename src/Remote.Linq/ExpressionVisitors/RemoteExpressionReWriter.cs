// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionVisitors
{
    using Aqua.TypeSystem;
    using Remote.Linq.Expressions;
    using System;
    using System.ComponentModel;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class RemoteExpressionReWriter
    {
        public static T ReplaceResourceDescriptorsByQueryable<T>(this T expression, ITypeResolver typeResolver = null, Func<Type, System.Linq.IQueryable> provider = null) where T : Expression
            => new QueryableResourceVisitor().ReplaceResourceDescriptorsByQueryable(expression, provider, typeResolver);

        public static T ReplaceNonGenericQueryArgumentsByGenericArguments<T>(this T expression) where T : Expression
            => new VariableQueryArgumentVisitor().ReplaceNonGenericQueryArgumentsByGenericArguments(expression);

        public static Expression ReplaceQueryableByResourceDescriptors(this Expression expression, ITypeInfoProvider typeInfoProvider = null)
            => new QueryableResourceVisitor().ReplaceQueryablesByResourceDescriptors(expression, typeInfoProvider);

        public static T ReplaceGenericQueryArgumentsByNonGenericArguments<T>(this T expression) where T : Expression
            => new VariableQueryArgumentVisitor().ReplaceGenericQueryArgumentsByNonGenericArguments(expression);
    }
}
