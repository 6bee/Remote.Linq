// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionVisitors;

using Aqua.TypeSystem;
using Remote.Linq.Expressions;
using System;
using System.ComponentModel;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class RemoteExpressionReWriter
{
    public static TExpression ReplaceResourceDescriptorsByQueryable<TExpression, TQueryable>(this TExpression expression, Func<Type, TQueryable> provider, ITypeResolver? typeResolver = null)
        where TExpression : Expression
        => QueryableResourceVisitor.ReplaceResourceDescriptorsByQueryable(expression, provider, typeResolver);

    public static TExpression ReplaceNonGenericQueryArgumentsByGenericArguments<TExpression>(this TExpression expression, ITypeResolver? typeResolver = null)
        where TExpression : Expression
        => VariableQueryArgumentVisitor.ReplaceNonGenericQueryArgumentsByGenericArguments(expression, typeResolver);

    public static Expression ReplaceQueryableByResourceDescriptors(this Expression expression, ITypeInfoProvider? typeInfoProvider = null, ITypeResolver? typeResolver = null)
        => QueryableResourceVisitor.ReplaceQueryablesByResourceDescriptors(expression, typeInfoProvider, typeResolver);

    public static TExpression ReplaceGenericQueryArgumentsByNonGenericArguments<TExpression>(this TExpression expression, ITypeResolver? typeResolver = null)
        where TExpression : Expression
        => VariableQueryArgumentVisitor.ReplaceGenericQueryArgumentsByNonGenericArguments(expression, typeResolver);
}