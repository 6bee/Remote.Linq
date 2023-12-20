// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery;

using Aqua.TypeSystem;
using System;
using System.Runtime.Serialization;

[Serializable]
[DataContract]
public sealed class SubstitutionValue
{
    public SubstitutionValue()
    {
    }

    public SubstitutionValue(Type type)
        : this(type.AsTypeInfo())
    {
    }

    public SubstitutionValue(TypeInfo type)
    {
        Type = type.CheckNotNull();
    }

    [DataMember(Name = "Type", Order = 1, IsRequired = true, EmitDefaultValue = false)]
    public TypeInfo Type { get; set; } = null!;

    public override string ToString() => $"{nameof(SubstitutionValue)}({Type})";
}