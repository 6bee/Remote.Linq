// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions;

using Aqua.TypeSystem;
using Remote.Linq.Text.Json.Converters;
using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[Serializable]
[DataContract]
[JsonConverter(typeof(ExpressionConverter<UnaryExpression>))]
public sealed class UnaryExpression : Expression
{
    public UnaryExpression()
    {
    }

    public UnaryExpression(UnaryOperator unaryOperator, Expression operand, TypeInfo type, MethodInfo? method)
    {
        UnaryOperator = unaryOperator;
        Operand = operand.CheckNotNull();
        Type = type.CheckNotNull();
        Method = method;
    }

    public UnaryExpression(UnaryOperator unaryOperator, Expression operand, Type type, System.Reflection.MethodInfo? method)
        : this(unaryOperator, operand, type.AsTypeInfo(), method.AsMethodInfo())
    {
    }

    public override ExpressionType NodeType => ExpressionType.Unary;

    [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = true)]
    public UnaryOperator UnaryOperator { get; set; }

    [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
    public Expression Operand { get; set; } = null!;

    [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false)]
    public TypeInfo Type { get; set; } = null!;

    [DataMember(Order = 4, IsRequired = false, EmitDefaultValue = false)]
    public MethodInfo? Method { get; set; }

    protected internal override ExpressionDebugFormatter DebugFormatter => new UnaryExpressionDebugView(this);

    public override string? ToString() => DebugFormatter.ToString();

    private sealed class UnaryExpressionDebugView : ExpressionDebugFormatter<UnaryExpression>
    {
        public UnaryExpressionDebugView(UnaryExpression expression)
            : base(expression)
        {
        }

        public override string ToString()
            => Expression.UnaryOperator switch
            {
                // TODO: [expression string formatter] add operation specific info/format
                UnaryOperator.Convert => $"{Expression.UnaryOperator}({Expression.Operand?.DebugFormatter}, {Format(Expression.Type)})",
                _ => $"{Expression.UnaryOperator}({Expression.Operand?.DebugFormatter})",
            };
    }
}