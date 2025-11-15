// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions;

using Aqua.EnumerableExtensions;
using Aqua.TypeSystem;
using Remote.Linq.Text.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[Serializable]
[DataContract]
[JsonConverter(typeof(ExpressionConverter<NewExpression>))]
public sealed class NewExpression : Expression
{
    public NewExpression()
    {
    }

    public NewExpression(TypeInfo type)
    {
        Type = type.CheckNotNull();
    }

    public NewExpression(Type type)
        : this(new TypeInfo(type))
    {
    }

    public NewExpression(ConstructorInfo constructor, IEnumerable<Expression>? arguments, IEnumerable<MemberInfo>? members = null)
        : this(constructor.CheckNotNull().DeclaringType ?? throw new ArgumentException($"{nameof(ConstructorInfo.DeclaringType)} not set", nameof(constructor)))
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
    public TypeInfo Type { get; set; } = null!;

    public override string ToString() => $"new {Constructor?.DeclaringType ?? Type}({Arguments.StringJoin(", ")})";
}