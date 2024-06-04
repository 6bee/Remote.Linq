// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#pragma warning disable IDE0130 // Namespance does not match folder structure
namespace System.Linq.Expressions;
#pragma warning restore IDE0130 // Namespance does not match folder structure

using Remote.Linq;
using Remote.Linq.DynamicQuery;
using System;
using System.ComponentModel;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class SystemExpressionFactoryExtensions
{
    public static MemberExpression CreateVariableQueryArgumentExpression<T>(this SystemExpressionFactory factory, T? value, Type? type = null)
    {
        type ??= value?.GetType() ?? typeof(T);
        return Expression.Property(
            Expression.New(
                typeof(VariableQueryArgument<>).MakeGenericType(type).GetConstructor([type])!,
                Expression.Constant(value, type)),
            nameof(VariableQueryArgument<object>.Value));
    }
}