// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.DynamicQuery;

using Aqua.TypeSystem;
using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

public static class RemoteLinqEfCoreAsyncQueryable
{
    private static ConcurrentDictionary<Type, Func<IRemoteLinqEfCoreAsyncQueryProvider, Expression?, IAsyncRemoteQueryable>> _genericConstructorCache = new();

    public static IAsyncRemoteQueryable CreateNonGeneric(IRemoteLinqEfCoreAsyncQueryProvider provider, Expression expression)
    {
        var elementType = TypeHelper.GetElementType(expression.Type)
          ?? throw new RemoteLinqException($"Cannot get element type based on expression's type {expression.Type}");

        return CreateNonGeneric(elementType, provider, expression);
    }

    public static IAsyncRemoteQueryable CreateNonGeneric(Type elementType, IRemoteLinqEfCoreAsyncQueryProvider provider, Expression? expression)
    {
        var genericConstructor = _genericConstructorCache.GetOrAdd(
            elementType,
            x =>
            {
                var providerParameter = Expression.Parameter(typeof(IRemoteLinqEfCoreAsyncQueryProvider), nameof(provider));
                var expressionParameter = Expression.Parameter(typeof(Expression), nameof(expression));

                var genericType = typeof(RemoteLinqEfCoreAsyncQueryable<>).MakeGenericType(elementType);
                var constructor = genericType.GetConstructors()[0];

                return Expression.Lambda<Func<IRemoteLinqEfCoreAsyncQueryProvider, Expression?, IAsyncRemoteQueryable>>(Expression.New(constructor, providerParameter, expressionParameter), providerParameter, expressionParameter).Compile();
            });

        return genericConstructor(provider, expression);
    }
}