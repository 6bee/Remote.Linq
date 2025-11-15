// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery;

using Aqua.Text.Json.Converters;
using Aqua.TypeSystem;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[Serializable]
[DataContract]
[JsonConverter(typeof(ObjectConverter<QueryableResourceDescriptor>))]
[QueryArgument]
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
        => Type = type.CheckNotNull();

    [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
    public TypeInfo Type { get; set; } = default!;

    public override string ToString() => $"{nameof(QueryableResourceDescriptor)}({Type})";
}