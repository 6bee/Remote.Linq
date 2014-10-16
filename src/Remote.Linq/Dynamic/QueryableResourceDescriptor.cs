// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.TypeSystem;
using System;
using System.Runtime.Serialization;

namespace Remote.Linq.Dynamic
{
    [Serializable]
    [DataContract]
    public sealed class QueryableResourceDescriptor
    {
        public QueryableResourceDescriptor(Type type)
            : this(new TypeInfo(type))
        {
        }

        public QueryableResourceDescriptor(TypeInfo type)
        {
            Type = type;
        }

        [DataMember(Name = "Type", IsRequired = true, EmitDefaultValue = false)]
        public TypeInfo Type { get; private set; }
    }
}
