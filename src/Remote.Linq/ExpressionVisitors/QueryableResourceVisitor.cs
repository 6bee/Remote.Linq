// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionVisitors
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using Aqua.TypeSystem.Extensions;
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.Expressions;
    using System;
    using System.Linq;
    using System.Reflection;

    public class QueryableResourceVisitor
    {
        internal T ReplaceResourceDescriptorsByQueryable<T>(T expression, Func<Type, IQueryable> provider, ITypeResolver typeResolver) where T : Expression
            => (T)new ResourceDescriptorVisitor(provider, typeResolver).ReplaceResourceDescriptorsByQueryable(expression);

        internal Expression ReplaceQueryablesByResourceDescriptors(Expression expression, ITypeInfoProvider typeInfoProvider)
            => new QueryableVisitor(typeInfoProvider).ReplaceQueryablesByResourceDescriptors(expression);

        protected class ResourceDescriptorVisitor : RemoteExpressionVisitorBase
        {
            private readonly ITypeResolver _typeResolver;
            private readonly Func<Type, IQueryable> _provider;

            internal protected ResourceDescriptorVisitor(Func<Type, IQueryable> provider, ITypeResolver typeResolver)
            {
                _provider = provider;
                _typeResolver = typeResolver ?? TypeResolver.Instance;
            }

            internal Expression ReplaceResourceDescriptorsByQueryable(Expression expression)
                => Visit(expression);

            protected override ConstantExpression VisitConstant(ConstantExpression expression)
            {
                var type = expression.Type.ResolveType(_typeResolver);
                {
                    if (type == typeof(QueryableResourceDescriptor) && expression.Value is QueryableResourceDescriptor queryableResourceDescriptor)
                    {
                        var queryableType = queryableResourceDescriptor.Type.ResolveType(_typeResolver);
                        var queryable = _provider(queryableType);
                        return new ConstantExpression(queryable);
                    }
                }

                if (type == typeof(ConstantQueryArgument) && !(expression.Value is null))
                {
                    var newConstantQueryArgument = new ConstantQueryArgument((ConstantQueryArgument)expression.Value);
                    var properties = newConstantQueryArgument.Properties ?? Enumerable.Empty<Property>();
                    foreach (var property in properties)
                    {
                        if (property.Value is QueryableResourceDescriptor queryableResourceDescriptor)
                        {
                            var queryableType = queryableResourceDescriptor.Type.ResolveType(_typeResolver);
                            var queryable = _provider(queryableType);
                            property.Value = queryable;
                        }
                    }

                    return new ConstantExpression(newConstantQueryArgument);
                }

                return base.VisitConstant(expression);
            }
        }

        protected class QueryableVisitor : RemoteExpressionVisitorBase
        {
            private readonly ITypeInfoProvider _typeInfoProvider;

            internal protected QueryableVisitor(ITypeInfoProvider typeInfoProvider)
            {
                _typeInfoProvider = typeInfoProvider ?? new TypeInfoProvider(false, false);
            }

            internal Expression ReplaceQueryablesByResourceDescriptors(Expression expression)
                => Visit(expression);

            protected override ConstantExpression VisitConstant(ConstantExpression expression)
            {
                var queryable = AsQueryableOrNull(expression.Value);
                if (!(queryable is null))
                {
                    var typeInfo = _typeInfoProvider.Get(queryable.ElementType);
                    var queryableResourceDescriptor = new QueryableResourceDescriptor(typeInfo);
                    return new ConstantExpression(queryableResourceDescriptor);
                }

                var constantQueryArgument = expression.Value as ConstantQueryArgument;
                if (!(constantQueryArgument is null))
                {
                    var newConstantQueryArgument = new ConstantQueryArgument(constantQueryArgument);
                    var properties = newConstantQueryArgument.Properties ?? Enumerable.Empty<Property>();
                    foreach (var property in properties)
                    {
                        var value = AsQueryableOrNull(property.Value);
                        if (!(value is null))
                        {
                            var typeInfo = _typeInfoProvider.Get(value.ElementType);
                            var queryableResourceDescriptor = new QueryableResourceDescriptor(typeInfo);
                            property.Value = queryableResourceDescriptor;
                        }
                    }

                    return new ConstantExpression(newConstantQueryArgument, expression.Type);
                }

                return base.VisitConstant(expression);
            }

            private static IQueryable AsQueryableOrNull(object value)
            {
                var queryable = value as IQueryable;
                if (queryable is null)
                {
                    return null;
                }

                var type = queryable.GetType();
                if (type.IsGenericType() && typeof(EnumerableQuery<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
                {
                    return null;
                }

                return queryable;
            }
        }
    }
}
