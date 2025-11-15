// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions;

using Remote.Linq.Text.Json.Converters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[Serializable]
[DataContract]
[JsonConverter(typeof(ExpressionConverter<LabelExpression>))]
public sealed class LabelExpression : Expression
{
    public LabelExpression()
    {
    }

    public LabelExpression(LabelTarget target, Expression? defaultValue)
    {
        Target = target.CheckNotNull();
        DefaultValue = defaultValue;
    }

    public override ExpressionType NodeType => ExpressionType.Label;

    [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
    public LabelTarget Target { get; set; } = null!;

    [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false)]
    public Expression? DefaultValue { get; set; }

    public override string ToString() => $".label {Target} {DefaultValue}";
}