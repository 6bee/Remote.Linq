// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions;

using Aqua.Text.Json.Converters;
using Aqua.TypeSystem;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[Serializable]
[DataContract]
[JsonConverter(typeof(ObjectConverter<ElementInit>))]
public sealed class ElementInit
{
    public ElementInit()
    {
    }

    public ElementInit(MethodInfo addMethod, IEnumerable<Expression> arguments)
    {
        AddMethod = addMethod.CheckNotNull();
        Arguments = [.. arguments.CheckNotNull()];
    }

    public ElementInit(System.Reflection.MethodInfo addMethod, IEnumerable<Expression> arguments)
        : this(new MethodInfo(addMethod), arguments)
    {
    }

    public ElementInit(string methodName, Type declaringType, Type[] genericArguments, Type[] parameterTypes, Type returnType, IEnumerable<Expression> arguments)
        : this(new MethodInfo(methodName, declaringType, genericArguments, parameterTypes, returnType), arguments)
    {
    }

    [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
    public MethodInfo AddMethod { get; set; } = null!;

    [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
    public List<Expression> Arguments { get; set; } = null!;

    public override string ToString()
        => $"{AddMethod?.Name}({string.Join(", ", Arguments ?? Enumerable.Empty<Expression>())})";
}