// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions;

using Remote.Linq.Text.Json.Converters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[Serializable]
[DataContract]
[JsonConverter(typeof(ExpressionConverter<ConditionalExpression>))]
public sealed class ConditionalExpression : Expression
{
    public ConditionalExpression()
    {
    }

    public ConditionalExpression(Expression test, Expression ifTrue, Expression ifFalse)
    {
        Test = test;
        IfTrue = ifTrue;
        IfFalse = ifFalse;
    }

    public override ExpressionType NodeType => ExpressionType.Conditional;

    [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
    public Expression Test { get; set; } = null!;

    [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
    public Expression IfTrue { get; set; } = null!;

    [DataMember(Order = 3, IsRequired = true, EmitDefaultValue = false)]
    public Expression IfFalse { get; set; } = null!;

    public override string ToString() => $"if {Test} then {IfTrue} else {IfFalse}";
}