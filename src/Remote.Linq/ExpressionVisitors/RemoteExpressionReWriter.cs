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
        public static T ReplaceResourceDescriptorsByQueryable<T>(this T expression, Func<Type, System.Linq.IQueryable> provider, ITypeResolver? typeResolver = null)
            where T : Expression
            => QueryableResourceVisitor.ReplaceResourceDescriptorsByQueryable(expression, provider, typeResolver);

        public static T ReplaceNonGenericQueryArgumentsByGenericArguments<T>(this T expression)
            where T : Expression
            => VariableQueryArgumentVisitor.ReplaceNonGenericQueryArgumentsByGenericArguments(expression);

        public static Expression ReplaceQueryableByResourceDescriptors(this Expression expression, ITypeInfoProvider? typeInfoProvider = null)
            => QueryableResourceVisitor.ReplaceQueryablesByResourceDescriptors(expression, typeInfoProvider);

        public static T ReplaceGenericQueryArgumentsByNonGenericArguments<T>(this T expression)
            where T : Expression
            => VariableQueryArgumentVisitor.ReplaceGenericQueryArgumentsByNonGenericArguments(expression);
    }
}
