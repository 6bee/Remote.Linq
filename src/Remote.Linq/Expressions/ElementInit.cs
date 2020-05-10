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

        public ElementInit(string methodName, Type declaringType, Type[] genericArguments, Type[] parameterTypes, Type returnType, IEnumerable<Expression> arguments, bool? isStatic)
            : this(new MethodInfo(methodName, declaringType, genericArguments, parameterTypes, returnType) { IsStatic = isStatic.NullIf(flag => !flag) }, arguments)
        {
        }

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public MethodInfo AddMethod { get; set; }

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
        public List<Expression> Arguments { get; set; }

        public override string ToString()
            => $"{AddMethod?.Name}({string.Join(", ", Arguments ?? Enumerable.Empty<Expression>())})";
    }
}
