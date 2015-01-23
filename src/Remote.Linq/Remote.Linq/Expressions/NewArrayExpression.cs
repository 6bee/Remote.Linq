// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Remote.Linq.TypeSystem;
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

        internal NewArrayExpression(TypeInfo typeInfo, IEnumerable<Expression> expressions)
        {
            Type = typeInfo;
            Expressions = expressions.ToList();
        }

        internal NewArrayExpression(Type type, IEnumerable<Expression> expressions)
            : this(new TypeInfo(type), expressions)
        {
        }

        public override ExpressionType NodeType { get { return ExpressionType.NewArray; } }

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public TypeInfo Type { get; set; }

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
        public List<Expression> Expressions { get; set; }

        public override string ToString()
        {
            return string.Format("New [] {{ {0} }}", Expressions.Select(x => x.ToString()).ToArray());
        }
    }
}
