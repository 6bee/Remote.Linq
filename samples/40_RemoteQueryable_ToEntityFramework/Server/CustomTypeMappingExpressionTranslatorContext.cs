// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server;

using Aqua.TypeSystem;
using Remote.Linq.EntityFramework;
using System;
using System.Linq;

/// <summary>
/// Implements type mappings from DTO model to entity model and vice versa.
/// </summary>
public sealed class CustomTypeMappingExpressionTranslatorContext : EntityFrameworkExpressionTranslatorContext
{
    private static readonly Type _dtoModelType = typeof(Common.Model.Product);
    private static readonly Type _entityModelType = typeof(Server.DbModel.ProductEntity);

    public CustomTypeMappingExpressionTranslatorContext()
        : base(new DtoModelToEntityModelMapper(), new EntityModelToDtoModelMapper())
    {
    }

    private sealed class DtoModelToEntityModelMapper : TypeResolver
    {
        public override Type ResolveType(TypeInfo type)
            => ResolveType(type, true);

        private Type ResolveType(TypeInfo type, bool mapGenericArguments)
        {
            if (string.Equals(type.Namespace, _dtoModelType.Namespace, StringComparison.Ordinal))
            {
                // map dto model to entity model
                var entityModelTypeName = $"{_entityModelType.Namespace}.{type.Name}Entity";
                return _entityModelType.Assembly.GetType(entityModelTypeName);
            }
            else if (mapGenericArguments && type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                var genericTypeArguments = type.GenericArguments
                    .Select(x =>
                    {
                        var t = ResolveType(x, true);
                        var includePropertyInfos = x.Properties?.Count > 0;
                        return new TypeInfo(t, includePropertyInfos, false);
                    })
                    .ToList();
                var copy = new TypeInfo(type) { GenericArguments = genericTypeArguments };
                return ResolveType(copy, false);
            }

            return base.ResolveType(type);
        }
    }

    private sealed class EntityModelToDtoModelMapper : TypeInfoProvider
    {
        public override TypeInfo GetTypeInfo(Type type, bool? includePropertyInfos = null, bool? setMemberDeclaringTypes = null)
        {
            if (string.Equals(type.Namespace, _entityModelType.Namespace, StringComparison.Ordinal))
            {
                // map entity model to dto model
                var dtoModelTypeName = $"{_dtoModelType.Namespace}.{type.Name.Substring(0, type.Name.Length - "Entity".Length)}";
                type = _dtoModelType.Assembly.GetType(dtoModelTypeName);
            }
            else if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                var genericTypeArguments = type.GenericTypeArguments.Select(x => (Type)GetTypeInfo(x, true, true)).ToArray();
                type = type.GetGenericTypeDefinition().MakeGenericType(genericTypeArguments);
            }

            return base.GetTypeInfo(type, includePropertyInfos, setMemberDeclaringTypes);
        }
    }
}