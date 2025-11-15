// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery;

using Aqua.TypeSystem;
using Remote.Linq.Text.Json.Converters;
using System.Collections;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

/// <summary>
/// This type is used to distinguish variable query arguments from constant query arguments.
/// </summary>
[Serializable]
[DataContract]
[JsonConverter(typeof(VariableQueryArgumentListConverter))]
[QueryArgument]
public sealed class VariableQueryArgumentList
{
    public VariableQueryArgumentList()
    {
    }

    public VariableQueryArgumentList(IEnumerable values, Type? elementType = null)
        : this(values, elementType.AsTypeInfo())
    {
    }

    public VariableQueryArgumentList(IEnumerable values, TypeInfo? elementType = null)
    {
        values.AssertNotNull();
        if (elementType is null)
        {
            var collectionType = values.GetType();
            var type = collectionType.GetElementTypeOrThrow();
            elementType = type.AsTypeInfo();
        }

        ElementType = elementType;

        Values = values.Cast<object?>().ToList();
    }

    [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
    public TypeInfo ElementType { get; set; } = null!;

    [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = true)]
    public List<object?> Values { get; set; } = null!;

    public override string ToString()
        => $"{nameof(VariableQueryArgumentList)}Of{ElementType?.Name ?? "?"}[{Values?.Count ?? 0}]";
}