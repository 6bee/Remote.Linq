// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class SwitchCase
    {
        public SwitchCase()
        {
        }

        public SwitchCase(Expression body, List<Expression> testValues)
        {
            Body = body ?? throw new ArgumentNullException(nameof(body));
            TestValues = testValues ?? throw new ArgumentNullException(nameof(testValues));
        }

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public Expression Body { get; set; } = null!;

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
        public List<Expression> TestValues { get; set; } = null!;
    }
}
