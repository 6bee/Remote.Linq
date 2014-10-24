// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.TypeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
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

        public override MemberBindingType BindingType { get { return MemberBindingType.MemberBinding; } }

        [DataMember(Order = 1)]
        public List<MemberBinding> Bindings { get; set; }
    }
}
