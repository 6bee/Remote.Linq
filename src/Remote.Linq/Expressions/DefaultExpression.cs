﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions;

using Aqua.TypeSystem;
using System;
using System.Runtime.Serialization;

[Serializable]
[DataContract]
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