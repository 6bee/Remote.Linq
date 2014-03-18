// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;

namespace Remote.Linq
{
    public partial class TypeResolver : ITypeResolver
    {
        private static readonly ITypeResolver _defaultTypeResolver = new TypeResolver();
        
        private TypeResolver()
        {
        }

        public static ITypeResolver Instance
        {
            get { return _instance ?? _defaultTypeResolver; }
            set { _instance = value; }
        }
        private static ITypeResolver _instance;

        public Type ResolveType(string typeName)
        {
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
    }
}
