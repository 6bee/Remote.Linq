// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions;

using Aqua.Text.Json.Converters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[Serializable]
[DataContract]
[JsonConverter(typeof(ObjectConverter<SortExpression>))]
public sealed class SortExpression
{
    public SortExpression()
    {
    }

    public SortExpression(LambdaExpression operand, SortDirection sortDirection)
    {
        Operand = operand.CheckNotNull();
        SortDirection = sortDirection;
    }

    [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
    public LambdaExpression Operand { get; set; } = null!;

    [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = true)]
    public SortDirection SortDirection { get; set; }

    public override string ToString() => $"orderby ({Operand}) {SortDirection}";
}