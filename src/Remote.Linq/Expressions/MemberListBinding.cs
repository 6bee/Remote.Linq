﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions;

using Aqua.TypeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

[Serializable]
[DataContract]
public sealed class MemberListBinding : MemberBinding
{
    public MemberListBinding()
    {
    }

    public MemberListBinding(MemberInfo member, IEnumerable<ElementInit> initializers)
        : base(member)
    {
        Initializers = initializers.CheckNotNull().ToList();
    }

    public override MemberBindingType BindingType => MemberBindingType.ListBinding;

    [DataMember(Order = 2)]
    public List<ElementInit> Initializers { get; set; } = null!;
}