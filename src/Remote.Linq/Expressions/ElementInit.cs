// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.TypeSystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using BindingFlags = System.Reflection.BindingFlags;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    public sealed class ElementInit
    {
        internal ElementInit(MethodInfo addMethod, IEnumerable<Expression> arguments)
        {
            AddMethod = addMethod;
            Arguments = arguments.ToList().AsReadOnly();
        }

        internal ElementInit(System.Reflection.MethodInfo addMethod, IEnumerable<Expression> arguments)
            : this(new MethodInfo(addMethod), arguments)
        {
        }

        internal ElementInit(string methodName, Type declaringType, BindingFlags bindingFlags, Type[] genericArguments, Type[] parameterTypes, IEnumerable<Expression> arguments)
            : this(new MethodInfo(methodName, declaringType, bindingFlags, genericArguments, parameterTypes), arguments)
        {
        }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public MethodInfo AddMethod { get; private set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public ReadOnlyCollection<Expression> Arguments { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}({1})", AddMethod.Name, string.Join(", ", Arguments.Select(x => x.ToString()).ToArray()));
        }
    }
}
