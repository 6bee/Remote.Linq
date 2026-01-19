// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions;

using Aqua.EnumerableExtensions;
using Remote.Linq.Text.Json.Converters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[Serializable]
[DataContract]
[JsonConverter(typeof(ExpressionConverter<ListInitExpression>))]
public sealed class ListInitExpression : Expression
{
    public ListInitExpression()
    {
    }

    public ListInitExpression(NewExpression newExpression, IEnumerable<ElementInit> initializers)
    {
        NewExpression = newExpression.CheckNotNull();
        Initializers = [.. initializers.CheckNotNull()];
    }

    public override ExpressionType NodeType => ExpressionType.ListInit;

    [DataMember(Order = 1)]
    public NewExpression NewExpression { get; set; } = null!;

    [DataMember(Order = 2)]
    public List<ElementInit> Initializers { get; set; } = null!;

    public override string ToString() => $"{NewExpression} {{ {Initializers.StringJoin(", ")} }}";
}