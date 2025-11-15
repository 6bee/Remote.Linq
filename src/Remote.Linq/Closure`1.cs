// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq;

using System.Runtime.Serialization;

[Serializable]
[DataContract]
public sealed class Closure<T>
{
    public Closure(T value)
        => Value = value;

    [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = true)]
    public T Value { get; init; }
}