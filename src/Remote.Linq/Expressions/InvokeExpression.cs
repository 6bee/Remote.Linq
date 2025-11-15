// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions;

using Remote.Linq.Text.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[Serializable]
[DataContract]
[JsonConverter(typeof(ExpressionConverter<InvokeExpression>))]
public sealed class InvokeExpression : Expression
{
    public InvokeExpression()
    {
    }

    public InvokeExpression(Expression expression, IEnumerable<Expression>? arguments)
    {
        Expression = expression.CheckNotNull();
        Arguments = arguments?.Any() is true ? arguments.ToList() : null;
    }

    public override ExpressionType NodeType => ExpressionType.Invoke;

    [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
    public Expression Expression { get; set; } = null!;

    [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false)]
    public List<Expression>? Arguments { get; set; }

    public override string ToString() => $".invoke {Expression}";
}