﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions;

using Aqua.TypeSystem;
using Remote.Linq.DynamicQuery;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

[Serializable]
[DataContract]
[KnownType(typeof(ConstantQueryArgument)), XmlInclude(typeof(ConstantQueryArgument))]
[KnownType(typeof(QueryableResourceDescriptor)), XmlInclude(typeof(QueryableResourceDescriptor))]
[KnownType(typeof(SubstitutionValue)), XmlInclude(typeof(SubstitutionValue))]
[KnownType(typeof(VariableQueryArgument)), XmlInclude(typeof(VariableQueryArgument))]
[KnownType(typeof(VariableQueryArgumentList)), XmlInclude(typeof(VariableQueryArgumentList))]
public sealed class ConstantExpression : Expression
{
    public ConstantExpression()
    {
    }

    public ConstantExpression(object? value, Type? type = null)
    {
        if (type is null)
        {
            if (value is null)
            {
                type = typeof(object);
            }
            else
            {
                type = value.GetType();
            }
        }

        Type = type.AsTypeInfo();
        Value = value;
    }

    public ConstantExpression(object? value, TypeInfo type)
    {
        Type = type.CheckNotNull();
        Value = value;
    }

    public override ExpressionType NodeType => ExpressionType.Constant;

    [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
    public TypeInfo Type { get; set; } = null!;

    [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = true)]
    public object? Value { get; set; }

    public override string ToString() => Value.QuoteValue();
}