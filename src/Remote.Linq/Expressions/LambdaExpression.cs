﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions;

using Aqua.EnumerableExtensions;
using Aqua.TypeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

[Serializable]
[DataContract]
public sealed class LambdaExpression : Expression
{
    public LambdaExpression()
    {
    }

    public LambdaExpression(Expression expression, IEnumerable<ParameterExpression>? parameters)
    {
        Expression = expression.CheckNotNull();
        Parameters = parameters.AsNullIfEmpty()?.ToList();
    }

    public LambdaExpression(TypeInfo? type, Expression expression, IEnumerable<ParameterExpression>? parameters)
        : this(expression, parameters)
    {
        Type = type;
    }

    public LambdaExpression(Type? type, Expression expression, IEnumerable<ParameterExpression>? parameters)
        : this(type.AsTypeInfo(), expression, parameters)
    {
    }

    public override ExpressionType NodeType => ExpressionType.Lambda;

    [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
    public Expression Expression { get; set; } = null!;

    [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false)]
    public List<ParameterExpression>? Parameters { get; set; }

    [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false)]
    public TypeInfo? Type { get; set; }

    public override string ToString()
    {
        var parameters = Parameters;
        var (open, close) = parameters?.Count is 1 ? (null, null) : ("(", ")");
        var parameterList = parameters.StringJoin(", ");
        return $"{open}{parameterList}{close} => {Expression}";
    }
}