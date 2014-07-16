// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    public sealed class MemberInitExpression : Expression
    {
        internal MemberInitExpression(NewExpression newExpression, IEnumerable<MemberBinding> bindings)
        {
            NewExpression = newExpression;
            Bindings = bindings.ToList().AsReadOnly();
        }

        public override ExpressionType NodeType { get { return ExpressionType.MemberInit; } }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public NewExpression NewExpression { get; private set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public ReadOnlyCollection<MemberBinding> Bindings { get; private set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", NewExpression, string.Join(", ", Bindings.Select(b => b.ToString()).ToArray()));
        }
    }
}
