// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionVisitors;

using Aqua.Dynamic;
using Aqua.EnumerableExtensions;
using Aqua.TypeSystem;
using Remote.Linq.DynamicQuery;
using Remote.Linq.Expressions;

public abstract class QueryableResourceVisitor
{
    internal static TExpression ReplaceResourceDescriptorsByQueryable<TExpression, TQueryable>(TExpression expression, Func<Type, TQueryable> provider, ITypeResolver? typeResolver)
        where TExpression : Expression
        => (TExpression)new ResourceDescriptorVisitor<TQueryable>(provider, typeResolver).Run(expression);

    internal static Expression ReplaceQueryablesByResourceDescriptors(Expression expression, ITypeInfoProvider? typeInfoProvider, ITypeResolver? typeResolver)
        => new QueryableVisitor(typeInfoProvider, typeResolver).Run(expression);

    protected class ResourceDescriptorVisitor<TQueryable> : RemoteExpressionVisitorBase
    {
        private readonly Func<Type, TQueryable> _provider;

        internal protected ResourceDescriptorVisitor(Func<Type, TQueryable> provider, ITypeResolver? typeResolver)
            : base(typeResolver)
            => _provider = provider.CheckNotNull();

        public Expression Run(Expression expression) => Visit(expression);

        protected override Expression VisitConstant(ConstantExpression node)
        {
            var value = node.CheckNotNull().Value;
            if (TryGetQueryableByQueryableResourceDescriptor(value, out var queryable))
            {
                return new ConstantExpression(queryable);
            }

            if (TryResolveQueryableSuorceInConstantQueryArgument(value, out var constantQueryArgument))
            {
                return new ConstantExpression(constantQueryArgument, node.Type);
            }

            if (TryResolveSubstitutedValue(value, out var substitutedValue))
            {
                return new ConstantExpression(substitutedValue, node.Type);
            }

            return base.VisitConstant(node);
        }

        private bool TryGetQueryableByQueryableResourceDescriptor(object? value, out TQueryable? queryable)
        {
            if (value is QueryableResourceDescriptor queryableResourceDescriptor)
            {
                var queryableType = queryableResourceDescriptor.Type.ResolveType(TypeResolver);
                queryable = _provider(queryableType);
                return true;
            }

            queryable = default;
            return false;
        }

        private bool TryResolveQueryableSuorceInConstantQueryArgument(object? value, out ConstantQueryArgument? newConstantQueryArgument)
        {
            if (value is ConstantQueryArgument constantQueryArgument)
            {
                var hasChanged = false;
                var tempConstantQueryArgument = new ConstantQueryArgument(constantQueryArgument.Value);
                foreach (var property in tempConstantQueryArgument.Value.Properties.AsEmptyIfNull())
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

        private bool TryResolveSubstitutedValue(object? value, out object? resubstitutionValue)
        {
            if (value is SubstitutionValue substitutionValue)
            {
                var type = substitutionValue.Type.ResolveType(TypeResolver);
                if (type == typeof(CancellationToken))
                {
                    // TODO: should/can cancellation token be retrieved from context?
                    resubstitutionValue = CancellationToken.None;
                    return true;
                }
            }

            resubstitutionValue = null;
            return false;
        }
    }

    protected class QueryableVisitor : RemoteExpressionVisitorBase
    {
        private readonly ITypeInfoProvider _typeInfoProvider;

        internal protected QueryableVisitor(ITypeInfoProvider? typeInfoProvider, ITypeResolver? typeResolver)
            : base(typeResolver)
            => _typeInfoProvider = typeInfoProvider ?? new TypeInfoProvider(false, false);

        public Expression Run(Expression expression) => Visit(expression);

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.CheckNotNull().Value.AsQueryableResourceTypeOrNull() is Type resourceType)
            {
                var typeInfo = _typeInfoProvider.GetTypeInfo(resourceType);
                var queryableResourceDescriptor = new QueryableResourceDescriptor(typeInfo);
                return new ConstantExpression(queryableResourceDescriptor);
            }

            if (node.Value is ConstantQueryArgument constantQueryArgument)
            {
                var copy = new DynamicObject(constantQueryArgument.Value);
                foreach (var property in copy.Properties.AsEmptyIfNull())
                {
                    if (property.Value.AsQueryableResourceTypeOrNull() is Type resourceTypePropertyValue)
                    {
                        var typeInfo = _typeInfoProvider.GetTypeInfo(resourceTypePropertyValue);
                        var queryableResourceDescriptor = new QueryableResourceDescriptor(typeInfo);
                        property.Value = queryableResourceDescriptor;
                    }
                }

                return new ConstantExpression(new ConstantQueryArgument(copy), node.Type);
            }

            if (node.Value is CancellationToken)
            {
                var substitutionValue = new SubstitutionValue(node.Type);
                return new ConstantExpression(substitutionValue, node.Type);
            }

            return base.VisitConstant(node);
        }
    }
}