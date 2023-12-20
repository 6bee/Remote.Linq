// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions;

using Aqua.EnumerableExtensions;
using Aqua.TypeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

[Serializable]
[DataContract]
public sealed class MethodCallExpression : Expression
{
    public MethodCallExpression()
    {
    }

    public MethodCallExpression(Expression? insatnce, MethodInfo methodInfo, IEnumerable<Expression>? arguments)
    {
        Instance = insatnce;
        Method = methodInfo;
        Arguments = arguments?.ToList();
    }

    public MethodCallExpression(Expression? insatnce, System.Reflection.MethodInfo methodInfo, IEnumerable<Expression>? arguments)
        : this(insatnce, new MethodInfo(methodInfo), arguments)
    {
    }

    public MethodCallExpression(Expression insatnce, string methodName, Type declaringType, Type[] genericArguments, Type[] parameterTypes, Type returnType, IEnumerable<Expression> arguments, bool? isStatic)
        : this(insatnce, new MethodInfo(methodName, declaringType, genericArguments, parameterTypes, returnType) { IsStatic = isStatic.NullIf(flag => !flag) }, arguments)
    {
    }

    public override ExpressionType NodeType => ExpressionType.Call;

    [DataMember(Order = 1, IsRequired = false, EmitDefaultValue = false)]
    public Expression? Instance { get; set; }

    [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
    public MethodInfo Method { get; set; } = null!;

    [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false)]
    public List<Expression>? Arguments { get; set; }

    public override string ToString()
    {
        var instalce = Instance is null ? null : $"{Instance}.";
        return $"{instalce}{Method?.Name}({Arguments.StringJoin(", ")})";
    }
}