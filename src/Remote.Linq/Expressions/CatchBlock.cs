﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions;

using Aqua.TypeSystem;
using System;
using System.Runtime.Serialization;

[Serializable]
[DataContract]
public sealed class CatchBlock
{
    public CatchBlock()
    {
    }

    public CatchBlock(Type test, ParameterExpression? parameter, Expression body, Expression? filter)
        : this(test.AsTypeInfo(), parameter, body, filter)
    {
    }

    public CatchBlock(TypeInfo test, ParameterExpression? parameter, Expression body, Expression? filter)
    {
        Test = test.CheckNotNull();
        Variable = parameter;
        Filter = filter;
        Body = body.CheckNotNull();
    }

    [DataMember(Order = 1, IsRequired = false, EmitDefaultValue = false)]
    public ParameterExpression? Variable { get; set; }

    [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false)]
    public Expression? Filter { get; set; }

    [DataMember(Order = 3, IsRequired = true, EmitDefaultValue = false)]
    public Expression Body { get; set; } = null!;

    [DataMember(Order = 4, IsRequired = true, EmitDefaultValue = false)]
    public TypeInfo Test { get; set; } = null!;

    public override string ToString()
    {
        string filter = Filter is null ? string.Empty : $" when ({Filter})";
        return $"catch({Test} {Variable}){filter} {Body}";
    }
}