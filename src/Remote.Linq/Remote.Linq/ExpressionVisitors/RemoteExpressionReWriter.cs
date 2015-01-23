// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionVisitors
{
    using Remote.Linq.Expressions;
    using Remote.Linq.TypeSystem;
    using System;
    using System.ComponentModel;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class RemoteExpressionReWriter
    {
        internal static Expression ReplaceResourceDescriptorsByQueryable(this Expression expression, ITypeResolver typeResolver = null, Func<Type, System.Linq.IQueryable> provider = null)
        {
            return new QueryableResourceVisitor(provider ?? ProviderRegistry.QueryableResourceProvider, typeResolver).ReplaceResourceDescriptorsByQueryable(expression);
        }

        internal static Expression ReplaceQueryableByResourceDescriptors(this Expression expression, ITypeResolver typeResolver = null)
        {
            return new QueryableResourceDescriptorVisitor(typeResolver).ReplaceQueryableResourceDescriptors(expression);
        }
    }
}
