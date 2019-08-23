// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class MemberMemberBinding : MemberBinding
    {
        public MemberMemberBinding()
            : base(null)
        {
        }

        public MemberMemberBinding(MemberInfo member, IEnumerable<MemberBinding> bindings)
            : base(member)
        {
            Bindings = bindings.ToList();
        }

        public override MemberBindingType BindingType => MemberBindingType.MemberBinding;

        [DataMember(Order = 1)]
        public List<MemberBinding> Bindings { get; set; }
    }
}
