// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions;

using Aqua.Text.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[Serializable]
[DataContract]
[JsonConverter(typeof(ObjectConverter<SwitchCase>))]
public sealed class SwitchCase
{
    public SwitchCase()
    {
    }

    public SwitchCase(Expression body, List<Expression> testValues)
    {
        Body = body.CheckNotNull();
        TestValues = testValues.CheckNotNull();
    }

    [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
    public Expression Body { get; set; } = null!;

    [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
    public List<Expression> TestValues { get; set; } = null!;
}