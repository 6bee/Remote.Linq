// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions;

using Aqua.TypeSystem;
using Remote.Linq.Text.Json.Converters;
using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[Serializable]
[DataContract]
[JsonConverter(typeof(ExpressionConverter<DefaultExpression>))]
public sealed class DefaultExpression : Expression
{
    public DefaultExpression()
    {
    }

    public DefaultExpression(Type type)
        : this(type.AsTypeInfo())
    {
    }

    public DefaultExpression(TypeInfo type)
    {
        Type = type.CheckNotNull();
    }

    public override ExpressionType NodeType => ExpressionType.Default;

    [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
    public TypeInfo Type { get; set; } = null!;

    public override string ToString() => $".default {Type}";
}