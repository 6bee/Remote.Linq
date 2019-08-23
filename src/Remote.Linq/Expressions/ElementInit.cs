// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using BindingFlags = System.Reflection.BindingFlags;

    [Serializable]
    [DataContract]
    public sealed class ElementInit
    {
        public ElementInit()
        {
        }

        public ElementInit(MethodInfo addMethod, IEnumerable<Expression> arguments)
        {
            AddMethod = addMethod;
            Arguments = arguments.ToList();
        }

        public ElementInit(System.Reflection.MethodInfo addMethod, IEnumerable<Expression> arguments)
            : this(new MethodInfo(addMethod), arguments)
        {
        }

        // TODO: replace binding flags by bool flags
        public ElementInit(string methodName, Type declaringType, BindingFlags bindingFlags, Type[] genericArguments, Type[] parameterTypes, IEnumerable<Expression> arguments)
            : this(new MethodInfo(methodName, declaringType, bindingFlags, genericArguments, parameterTypes), arguments)
        {
        }

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public MethodInfo AddMethod { get; set; }

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
        public List<Expression> Arguments { get; set; }

        public override string ToString()
            => string.Format("{0}({1})", AddMethod.Name, string.Join(", ", Arguments));
    }
}
