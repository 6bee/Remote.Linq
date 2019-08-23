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
    public sealed class MethodCallExpression : Expression
    {
        public MethodCallExpression()
        {
        }

        public MethodCallExpression(Expression insatnce, MethodInfo methodInfo, IEnumerable<Expression> arguments)
        {
            Instance = insatnce;
            Method = methodInfo;
            Arguments = arguments.ToList();
        }

        public MethodCallExpression(Expression insatnce, System.Reflection.MethodInfo methodInfo, IEnumerable<Expression> arguments)
            : this(insatnce, new MethodInfo(methodInfo), arguments)
        {
        }

        public MethodCallExpression(Expression insatnce, string methodName, Type declaringType, BindingFlags bindingFlags, Type[] genericArguments, Type[] parameterTypes, IEnumerable<Expression> arguments)
            : this(insatnce, new MethodInfo(methodName, declaringType, bindingFlags, genericArguments, parameterTypes), arguments)
        {
        }

        public override ExpressionType NodeType => ExpressionType.Call;

        [DataMember(Order = 1, IsRequired = false, EmitDefaultValue = false)]
        public Expression Instance { get; set; }

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
        public MethodInfo Method { get; set; }

        [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false)]
        public List<Expression> Arguments { get; set; }

        public override string ToString()
        {
            var instance = Instance;
            var arguments = Arguments;
            return string.Format(
                "{0}.{1}({2})",
                instance?.ToString() ?? Method?.DeclaringType?.ToString(),
                Method?.Name,
                arguments is null ? null : string.Join(", ", arguments));
        }
    }
}
