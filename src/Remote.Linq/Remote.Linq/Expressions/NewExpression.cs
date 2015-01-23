// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Remote.Linq.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using BindingFlags = System.Reflection.BindingFlags;

    [Serializable]
    [DataContract]
    public sealed class NewExpression : Expression
    {
        public NewExpression()
        {
        }

        internal NewExpression(ConstructorInfo constructor, IEnumerable<Expression> arguments, IEnumerable<MemberInfo> members = null)
        {
            Constructor = constructor;
            Arguments = ReferenceEquals(null, arguments) || !arguments.Any() ? null : arguments.ToList();
            Members = ReferenceEquals(null, members) || !members.Any() ? null : members.ToList();
        }

        internal NewExpression(System.Reflection.ConstructorInfo constructor, IEnumerable<Expression> arguments = null, IEnumerable<System.Reflection.MemberInfo> members = null)
            : this(new ConstructorInfo(constructor), arguments, ReferenceEquals(null, members) ? null : members.Select(x => MemberInfo.Create(x)))
        {
        }

        internal NewExpression(string name, Type declaringType, BindingFlags bindingFlags, Type[] genericArguments, Type[] parameterTypes, IEnumerable<Expression> arguments = null, IEnumerable<System.Reflection.MemberInfo> members = null)
            : this(new ConstructorInfo(name, declaringType, bindingFlags, genericArguments, parameterTypes), arguments)
        {
        }

        public override ExpressionType NodeType { get { return ExpressionType.New; } }

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public ConstructorInfo Constructor { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = true, IsRequired = false)]
        public List<Expression> Arguments { get; set; }

        [DataMember(Order = 3, EmitDefaultValue = true, IsRequired = false)]
        public List<MemberInfo> Members { get; set; }

        public override string ToString()
        {
            return string.Format("New {0}({1})", Constructor.DeclaringType, ReferenceEquals(null, Arguments) ? null : string.Join(", ", Arguments.Select(x => x.ToString()).ToArray()));
        }
    }
}
