// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions;

using Aqua.Text.Json.Converters;
using Aqua.TypeSystem;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[Serializable]
[DataContract]
[JsonConverter(typeof(ObjectConverter<MemberListBinding>))]
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