// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions;

using Aqua.EnumerableExtensions;
using Aqua.TypeSystem;
using Remote.Linq.Text.Json.Converters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[Serializable]
[DataContract]
[JsonConverter(typeof(ExpressionConverter<NewArrayExpression>))]
public sealed class NewArrayExpression : Expression
{
    public NewArrayExpression()
    {
    }

    public NewArrayExpression(NewArrayType newArrayType, TypeInfo typeInfo, IEnumerable<Expression> expressions)
    {
        NewArrayType = newArrayType;
        Type = typeInfo.CheckNotNull();
        Expressions = expressions.CheckNotNull().ToList();
    }

    public NewArrayExpression(NewArrayType newArrayType, Type type, IEnumerable<Expression> expressions)
        : this(newArrayType, new TypeInfo(type, false, false), expressions)
    {
    }

    public override ExpressionType NodeType => ExpressionType.NewArray;

    [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = true)]
    public NewArrayType NewArrayType { get; set; }

    [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
    public TypeInfo Type { get; set; } = null!;

    [DataMember(Order = 3, IsRequired = true, EmitDefaultValue = false)]
    public List<Expression> Expressions { get; set; } = null!;

    public override string ToString()
        => NewArrayType == NewArrayType.NewArrayBounds
        ? $"new {Type}[lenght]"
        : $"new {Type}[] {{ {Expressions.StringJoin(", ")} }}";
}