// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Linq;

namespace Remote.Linq.TypeSystem
{
    public partial class TypeResolver : ITypeResolver
    {
        private static readonly ITypeResolver _defaultTypeResolver = new TypeResolver();
        
        protected TypeResolver()
        {
        }

        public static ITypeResolver Instance
        {
            get { return _instance ?? _defaultTypeResolver; }
            set { _instance = value; }
        }
        private static ITypeResolver _instance;

        protected virtual Type ResolveType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) throw new ArgumentNullException("typeName", "Expected a valid type name");

            var type = Type.GetType(typeName);
            if (ReferenceEquals(null, type))
            {
                var assemblies = GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    type = assembly.GetType(typeName);
                    if (!ReferenceEquals(null, type)) break;
                }
                if (ReferenceEquals(null, type))
                {
                    throw new Exception(string.Format("Type '{0}' could not be resolved", typeName));
                }
            }
            return type;
        }

        public virtual Type ResolveType(TypeInfo typeInfo)
        {
            var type = ResolveType(typeInfo.FullName);
            if (typeInfo.IsGenericType)
            {
                var generigArguments =
                    from x in typeInfo.GenericArguments
                    select ResolveType(x);
                type = type.MakeGenericType(generigArguments.ToArray());
            }
            return type;
        }
    }
}
