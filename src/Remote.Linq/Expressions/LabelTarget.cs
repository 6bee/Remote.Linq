// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions;

using Aqua.Text.Json.Converters;
using Aqua.TypeSystem;
using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[Serializable]
[DataContract]
[JsonConverter(typeof(ObjectConverter<LabelTarget>))]
public sealed class LabelTarget
{
    public LabelTarget()
    {
    }

    public LabelTarget(string? name, Type? type = null, int instanceId = 0)
        : this(name, type.AsTypeInfo(), instanceId)
    {
    }

    public LabelTarget(string? name, TypeInfo? type, int instanceId)
    {
        Name = name;
        Type = type;
        InstanceId = instanceId;
    }

    [DataMember(Order = 1, IsRequired = false, EmitDefaultValue = false)]
    public string? Name { get; set; }

    [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false)]
    public TypeInfo? Type { get; set; }

    /// <summary>
    /// Gets or sets the instance id,
    /// which is used to denote a specific instance of <see cref="System.Linq.Expressions.ParameterExpression"/> within an expression tree.
    /// </summary>
    [DataMember(Order = 3, IsRequired = true, EmitDefaultValue = false)]
    public int InstanceId { get; set; }

    public override string ToString() => Name ?? InstanceId.ToString();
}