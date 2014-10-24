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
    public sealed class MemberListBinding : MemberBinding
    {
        public MemberListBinding()
            : base(null)
        {
        }

        public MemberListBinding(MemberInfo member, IEnumerable<ElementInit> initializers)
            : base(member)
        {
            Initializers = initializers.ToList();
        }

        public override MemberBindingType BindingType { get { return MemberBindingType.ListBinding; } }

        [DataMember(Order = 1)]
        public List<ElementInit> Initializers { get; set; }
    }
}
