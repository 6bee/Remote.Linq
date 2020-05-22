// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.Extensions;
    using Aqua.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class NewExpression : Expression
    {
        public NewExpression()
        {
        }

        public NewExpression(TypeInfo type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public NewExpression(Type type)
            : this(new TypeInfo(type))
        {
        }

        public NewExpression(ConstructorInfo constructor, IEnumerable<Expression>? arguments, IEnumerable<MemberInfo>? members = null)
            : this(constructor.DeclaringType ?? throw new ArgumentNullException(nameof(constructor)))
        {
            Constructor = constructor;
            Arguments = arguments?.ToList();
            Members = members?.ToList();
        }

        public NewExpression(System.Reflection.ConstructorInfo constructor, IEnumerable<Expression>? arguments = null, IEnumerable<System.Reflection.MemberInfo>? members = null)
            : this(new ConstructorInfo(constructor), arguments, members?.Select(x => MemberInfo.Create(x)))
        {
        }

        public NewExpression(string name, Type declaringType, IEnumerable<Type>? parameterTypes, IEnumerable<Expression>? arguments = null, IEnumerable<System.Reflection.MemberInfo>? members = null)
            : this(new ConstructorInfo(name, declaringType, parameterTypes), arguments, members?.Select(x => MemberInfo.Create(x)))
        {
        }

        public override ExpressionType NodeType => ExpressionType.New;

        [DataMember(Order = 1, IsRequired = false, EmitDefaultValue = false)]
        public ConstructorInfo? Constructor { get; set; }

        [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false)]
        public List<Expression>? Arguments { get; set; }

        [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false)]
        public List<MemberInfo>? Members { get; set; }

        [DataMember(Order = 4, IsRequired = false, EmitDefaultValue = false)]
        public TypeInfo? Type { get; set; }

        public override string ToString() => $"new {Constructor?.DeclaringType ?? Type}({Arguments.StringJoin(", ")})";
    }
}
