// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions;

using Aqua.Text.Json.Converters;
using Aqua.TypeSystem;
using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[Serializable]
[DataContract]
[JsonConverter(typeof(ObjectConverter<MemberAssignment>))]
public sealed class MemberAssignment : MemberBinding
{
    public MemberAssignment()
    {
    }

    public MemberAssignment(MemberInfo member, Expression expression)
        : base(member)
    {
        Expression = expression.CheckNotNull();
    }

    public override MemberBindingType BindingType => MemberBindingType.Assignment;

    [DataMember(Order = 2, IsRequired = true)]
    public Expression Expression { get; set; } = null!;
}