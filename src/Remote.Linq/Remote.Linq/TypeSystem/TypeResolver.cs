// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.TypeSystem
{
    using System;
    using System.Linq;

    public partial class TypeResolver : ITypeResolver
    {
        private static readonly ITypeResolver _defaultTypeResolver = new TypeResolver();
        private static ITypeResolver _instance;

        private readonly TransparentCache<TypeInfo, Type> _typeCache;
        private readonly TransparentCache<string, Type> _typeCacheByName;

        protected TypeResolver()
        {
            _typeCache = new TransparentCache<TypeInfo, Type>();
            _typeCacheByName = new TransparentCache<string, Type>();
        }

        /// <summary>
        /// Sets or gets an instance of ITypeResolver.
        /// </summary>
        /// <remarks>
        /// Setting this property allows for registring a custom type resolver. 
        /// Setting this property to null makes it fall-back to the default resolver.
        /// </remarks>
        public static ITypeResolver Instance
        {
            get { return _instance ?? _defaultTypeResolver; }
            set { _instance = value; }
        }

        public virtual Type ResolveType(TypeInfo typeInfo)
        {
            return _typeCache.GetOrCreate(typeInfo, ResolveTypeInternal);
        }

        public virtual Type ResolveType(string typeName)
        {
            return _typeCacheByName.GetOrCreate(typeName, ResolveTypeInternal);
        }

        private Type ResolveTypeInternal(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentNullException("typeName", "Expected a valid type name");
            }

            var type = Type.GetType(typeName);
            if (ReferenceEquals(null, type))
            {
                var assemblies = GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    type = assembly.GetType(typeName);

                    if (!ReferenceEquals(null, type))
                    {
                        break;
                    }
                }
            }

            return type;
        }

        private Type ResolveTypeInternal(TypeInfo typeInfo)
        {
            var type = Type.GetType(typeInfo.FullName);
            if (!IsValid(type, typeInfo))
            {
                var assemblies = GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    type = assembly.GetType(typeInfo.FullName);

                    if (IsValid(type, typeInfo))
                    {
                        break;
                    }

                    type = null;
                }
            }

#if NET
            if (ReferenceEquals(null, type))
            {
                type = EmitType(typeInfo);
            }
#endif

            if (ReferenceEquals(null, type))
            {
                throw new Exception(string.Format("Type '{0}' could not be resolved", typeInfo.FullName));
            }

            if (typeInfo.IsGenericType)
            {
                var generigArguments =
                    from x in typeInfo.GenericArguments
                    select ResolveType(x);

                if (typeInfo.IsArray)
                {
                    type = type.GetElementType().MakeGenericType(generigArguments.ToArray()).MakeArrayType();
                }
                else
                {
                    type = type.MakeGenericType(generigArguments.ToArray());
                }
            }

            return type;
        }

#if NET
        protected virtual Type EmitType(TypeInfo typeInfo)
        {
            var type = new Emit.TypeEmitter().EmitType(typeInfo);
            return type;
        }
#endif

        private static bool IsValid(Type type, TypeInfo typeInfo)
        {
            if (!ReferenceEquals(null, type))
            {
                if (typeInfo.IsArray)
                {
                    type = type.GetElementType();
                }

                if (typeInfo.IsAnonymousType || type.IsAnonymousType())
                {
                    var properties = type.GetProperties().Select(x => x.Name).ToList();
                    var propertyNames = typeInfo.Properties.Select(x => x.Name).ToList();

                    var match = 
                        type.IsAnonymousType() && 
                        typeInfo.IsAnonymousType && 
                        properties.Count == propertyNames.Count && 
                        propertyNames.All(x => properties.Contains(x));

                    if (!match)
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}
