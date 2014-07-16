// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.TypeSystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    public sealed class MemberMemberBinding : MemberBinding
    {
        public MemberMemberBinding(MemberInfo member, IEnumerable<MemberBinding> bindings)
            : base(member)
        {
            Bindings = bindings.ToList().AsReadOnly();
        }

        public override MemberBindingType BindingType { get { return MemberBindingType.MemberBinding; } }

        [DataMember]
        public ReadOnlyCollection<MemberBinding> Bindings { get; private set; }
    }
}
