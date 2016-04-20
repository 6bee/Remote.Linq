// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua;
    using Aqua.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using BindingFlags = System.Reflection.BindingFlags;

    [Serializable]
    [DataContract]
    public sealed class MethodCallExpression : Expression
    {
        public MethodCallExpression()
        {
        }

        internal MethodCallExpression(Expression insatnce, MethodInfo methodInfo, IEnumerable<Expression> arguments)
        {
            Instance = insatnce;
            Method = methodInfo;
            Arguments = arguments.ToList();
        }

        internal MethodCallExpression(Expression insatnce, System.Reflection.MethodInfo methodInfo, IEnumerable<Expression> arguments)
            : this(insatnce, new MethodInfo(methodInfo), arguments)
        {
        }

        internal MethodCallExpression(Expression insatnce, string methodName, Type declaringType, BindingFlags bindingFlags, Type[] genericArguments, Type[] parameterTypes, IEnumerable<Expression> arguments)
            : this(insatnce, new MethodInfo(methodName, declaringType, bindingFlags, genericArguments, parameterTypes), arguments)
        {
        }

        public override ExpressionType NodeType { get { return ExpressionType.MethodCall; } }

        [DataMember(Order = 1, IsRequired = false, EmitDefaultValue = false)]
        public Expression Instance { get; set; }

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
        public MethodInfo Method { get; set; }

        [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false)]
        public List<Expression> Arguments { get; set; }

        public override string ToString()
        {
            return string.Format("{0}.{1}({2})",
                Instance == null ? Method.DeclaringType.ToString() : Instance.ToString(),
                Method.Name,
                // ParameterTypes == null ? null : string.Join(", ", ParameterTypes)
                ReferenceEquals(null, Arguments) ? null : string.Join(", ", Arguments.Select(a => a.ToString()).ToArray()));
        }
    }
}
