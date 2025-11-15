// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions;

using Aqua.EnumerableExtensions;
using Remote.Linq.Text.Json.Converters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[Serializable]
[DataContract]
[JsonConverter(typeof(ExpressionConverter<MemberInitExpression>))]
public sealed class MemberInitExpression : Expression
{
    public MemberInitExpression()
    {
    }

    public MemberInitExpression(NewExpression newExpression, IEnumerable<MemberBinding> bindings)
    {
        NewExpression = newExpression.CheckNotNull();
        Bindings = bindings.CheckNotNull().ToList();
    }

    public override ExpressionType NodeType => ExpressionType.MemberInit;

    [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
    public NewExpression NewExpression { get; set; } = null!;

    [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
    public List<MemberBinding> Bindings { get; set; } = null!;

    public override string ToString() => $"{NewExpression} {Bindings.StringJoin(", ")}";
}