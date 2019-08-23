// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class NewArrayExpression : Expression
    {
        public NewArrayExpression()
        {
        }

        public NewArrayExpression(NewArrayType newArrayType, TypeInfo typeInfo, IEnumerable<Expression> expressions)
        {
            NewArrayType = newArrayType;
            Type = typeInfo;
            Expressions = expressions.ToList();
        }

        public NewArrayExpression(NewArrayType newArrayType, Type type, IEnumerable<Expression> expressions)
            : this(newArrayType, new TypeInfo(type, false, false), expressions)
        {
        }

        public override ExpressionType NodeType => ExpressionType.NewArray;

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = true)]
        public NewArrayType NewArrayType { get; set; }

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
        public TypeInfo Type { get; set; }

        [DataMember(Order = 3, IsRequired = true, EmitDefaultValue = false)]
        public List<Expression> Expressions { get; set; }

        public override string ToString()
        {
            return NewArrayType == NewArrayType.NewArrayBounds
                ? $"New [lenght]"
                : $"New [] {{ {string.Join(", ", Expressions)} }}";
        }
    }
}
