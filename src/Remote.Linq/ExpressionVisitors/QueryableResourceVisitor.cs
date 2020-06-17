// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionVisitors
{
    using Aqua.Dynamic;
    using Aqua.Extensions;
    using Aqua.TypeSystem;
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.Expressions;
    using System;
    using System.Linq;

    public static class QueryableResourceVisitor
    {
        internal static T ReplaceResourceDescriptorsByQueryable<T>(T expression, Func<Type, IQueryable> provider, ITypeResolver? typeResolver)
            where T : Expression
            => (T)new ResourceDescriptorVisitor(provider, typeResolver).Run(expression);

        internal static Expression ReplaceQueryablesByResourceDescriptors(Expression expression, ITypeInfoProvider? typeInfoProvider)
            => new QueryableVisitor(typeInfoProvider).Run(expression);

        protected class ResourceDescriptorVisitor : RemoteExpressionVisitorBase
        {
            private readonly ITypeResolver _typeResolver;
            private readonly Func<Type, IQueryable> _provider;

            internal protected ResourceDescriptorVisitor(Func<Type, IQueryable> provider, ITypeResolver? typeResolver)
            {
                _provider = provider;
                _typeResolver = typeResolver ?? TypeResolver.Instance;
            }

            internal Expression Run(Expression expression) => Visit(expression);

            protected override ConstantExpression VisitConstant(ConstantExpression node)
            {
                var value = node.CheckNotNull(nameof(node)).Value;
                if (TryGetQueryableByQueryableResourceDescriptor(value, out var queryable))
                {
                    return new ConstantExpression(queryable);
                }

                if (TryResolveQueryableSuorceInConstantQueryArgument(value, out var constantQueryArgument))
                {
#pragma warning disable CA1062 // Validate arguments of public methods -> false positive.
                    return new ConstantExpression(constantQueryArgument, node.Type);
#pragma warning restore CA1062 // Validate arguments of public methods
                }

                return base.VisitConstant(node);
            }

            private bool TryGetQueryableByQueryableResourceDescriptor(object? value, out IQueryable? queryable)
            {
                if (value is QueryableResourceDescriptor queryableResourceDescriptor)
                {
                    var queryableType = queryableResourceDescriptor.Type.ResolveType(_typeResolver);
                    queryable = _provider(queryableType);
                    return true;
                }

                queryable = null;
                return false;
            }

            private bool TryResolveQueryableSuorceInConstantQueryArgument(object? value, out ConstantQueryArgument? newConstantQueryArgument)
            {
                if (value is ConstantQueryArgument constantQueryArgument)
                {
                    var hasChanged = false;
                    var tempConstantQueryArgument = new ConstantQueryArgument(constantQueryArgument);
                    foreach (var property in tempConstantQueryArgument.Properties.AsEmptyIfNull())
                    {
                        if (TryGetQueryableByQueryableResourceDescriptor(property.Value, out var queryable))
                        {
                            property.Value = queryable;
                            hasChanged = true;
                        }
                    }

                    if (hasChanged)
                    {
                        newConstantQueryArgument = tempConstantQueryArgument;
                        return true;
                    }
                }

                newConstantQueryArgument = null;
                return false;
            }
        }

        protected class QueryableVisitor : RemoteExpressionVisitorBase
        {
            private readonly ITypeInfoProvider _typeInfoProvider;

            internal protected QueryableVisitor(ITypeInfoProvider? typeInfoProvider)
            {
                _typeInfoProvider = typeInfoProvider ?? new TypeInfoProvider(false, false);
            }

            internal Expression Run(Expression expression)
                => Visit(expression);

            protected override ConstantExpression VisitConstant(ConstantExpression node)
            {
                var queryable = node.CheckNotNull(nameof(node)).Value.AsQueryableOrNull();
                if (queryable != null)
                {
                    var typeInfo = _typeInfoProvider.GetTypeInfo(queryable.ElementType);
                    var queryableResourceDescriptor = new QueryableResourceDescriptor(typeInfo);
                    return new ConstantExpression(queryableResourceDescriptor);
                }

                if (node.Value is ConstantQueryArgument constantQueryArgument)
                {
                    var newConstantQueryArgument = new ConstantQueryArgument(constantQueryArgument);
                    var properties = newConstantQueryArgument.Properties ?? Enumerable.Empty<Property>();
                    foreach (var property in properties)
                    {
                        var value = property.Value.AsQueryableOrNull();
                        if (value != null)
                        {
                            var typeInfo = _typeInfoProvider.GetTypeInfo(value.ElementType);
                            var queryableResourceDescriptor = new QueryableResourceDescriptor(typeInfo);
                            property.Value = queryableResourceDescriptor;
                        }
                    }

                    return new ConstantExpression(newConstantQueryArgument, node.Type);
                }

                return base.VisitConstant(node);
            }
        }
    }
}
