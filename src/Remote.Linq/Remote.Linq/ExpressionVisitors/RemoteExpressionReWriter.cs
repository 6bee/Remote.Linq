// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionVisitors
{
    using Remote.Linq.Expressions;
    using Remote.Linq.TypeSystem;
    using System;
    using System.ComponentModel;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class RemoteExpressionReWriter
    {
        public static T ReplaceResourceDescriptorsByQueryable<T>(this T expression, ITypeResolver typeResolver = null, Func<Type, System.Linq.IQueryable> provider = null) where T : Expression
        {
            return new QueryableResourceVisitor().ReplaceResourceDescriptorsByQueryable(expression, provider ?? ProviderRegistry.QueryableResourceProvider, typeResolver);
        }

        public static T ReplaceNonGenericQueryArgumentsByGenericArguments<T>(this T expression) where T : Expression
        {
            return new VariableQueryArgumentVisitor().ReplaceNonGenericQueryArgumentsByGenericArguments(expression);
        }

        public static Expression ReplaceQueryableByResourceDescriptors(this Expression expression, ITypeResolver typeResolver = null)
        {
            return new QueryableResourceVisitor().ReplaceQueryablesByResourceDescriptors(expression, typeResolver);
        }

        public static T ReplaceGenericQueryArgumentsByNonGenericArguments<T>(this T expression) where T : Expression
        {
            return new VariableQueryArgumentVisitor().ReplaceGenericQueryArgumentsByNonGenericArguments(expression);
        }
    }
}
