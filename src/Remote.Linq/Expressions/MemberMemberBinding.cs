// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions;

using Aqua.Text.Json.Converters;
using Aqua.TypeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[Serializable]
[DataContract]
[JsonConverter(typeof(ObjectConverter<MemberMemberBinding>))]
public sealed class MemberMemberBinding : MemberBinding
{
    public MemberMemberBinding()
    {
    }

    public MemberMemberBinding(MemberInfo member, IEnumerable<MemberBinding> bindings)
        : base(member)
    {
        Bindings = bindings.CheckNotNull().ToList();
    }

    public override MemberBindingType BindingType => MemberBindingType.MemberBinding;

    [DataMember(Order = 2)]
    public List<MemberBinding> Bindings { get; set; } = null!;
}