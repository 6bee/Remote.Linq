// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.TypeSystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    public sealed class NewArrayExpression : Expression
    {
        internal NewArrayExpression(TypeInfo typeInfo, IEnumerable<Expression> expressions)
        {
            Type = typeInfo;
            Expressions = expressions.ToList().AsReadOnly();
        }

        internal NewArrayExpression(Type type, IEnumerable<Expression> expressions)
            : this(new TypeInfo(type), expressions)
        {
        }

        public override ExpressionType NodeType { get { return ExpressionType.NewArray; } }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public TypeInfo Type { get; private set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public ReadOnlyCollection<Expression> Expressions { get; private set; }

        public override string ToString()
        {
            return string.Format("New [] {{ {0} }}", Expressions.Select(x => x.ToString()).ToArray());
        }
    }
}
