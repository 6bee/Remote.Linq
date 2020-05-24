﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Aqua.TypeSystem;
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class QueryableResourceDescriptor
    {
        public QueryableResourceDescriptor()
        {
        }

        public QueryableResourceDescriptor(Type type)
            : this(type.AsTypeInfo())
        {
        }

        public QueryableResourceDescriptor(TypeInfo type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        [DataMember(Name = "Type", Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public TypeInfo Type { get; set; } = null!;
    }
}
