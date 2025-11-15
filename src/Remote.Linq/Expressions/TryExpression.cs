// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions;

using Aqua.EnumerableExtensions;
using Aqua.TypeSystem;
using Remote.Linq.Text.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[Serializable]
[DataContract]
[JsonConverter(typeof(ExpressionConverter<TryExpression>))]
public sealed class TryExpression : Expression
{
    public TryExpression()
    {
    }

    public TryExpression(Type type, Expression body, Expression? fault, Expression? @finally, List<CatchBlock>? handlers)
        : this(type.AsTypeInfo(), body, fault, @finally, handlers)
    {
    }

    public TryExpression(TypeInfo type, Expression body, Expression? fault, Expression? @finally, IEnumerable<CatchBlock>? handlers)
    {
        Type = type.CheckNotNull();
        Body = body.CheckNotNull();
        Handlers = handlers?.ToList();
        Finally = @finally;
        Fault = fault;
    }

    public override ExpressionType NodeType => ExpressionType.Try;

    [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
    public Expression Body { get; set; } = null!;

    [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false)]
    public List<CatchBlock>? Handlers { get; set; }

    [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false)]
    public Expression? Finally { get; set; }

    [DataMember(Order = 4, IsRequired = false, EmitDefaultValue = false)]
    public Expression? Fault { get; set; }

    [DataMember(Order = 5, IsRequired = false, EmitDefaultValue = false)]
    public TypeInfo Type { get; set; } = null!;

    public override string ToString()
        => $"try({Type}) {{{Body}}}" +
            (Handlers is null ? null : " " + Handlers.StringJoin("; ")) +
            (Finally is null ? null : $" finally{{{Finally}}}") +
            (Fault is null ? null : $" faulted{{{Fault}}}");
}