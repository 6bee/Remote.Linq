// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
#if !SILVERLIGHT
    [Serializable]
#endif
    [DataContract]
    public sealed class ConstantValueExpression : Expression
    {
        internal ConstantValueExpression(object value)
        {
            Value = value;
        }

        public override ExpressionType NodeType { get { return ExpressionType.ConstantValue; } }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public object Value { get; private set; }

        public override string ToString()
        {
            return string.Format("{1}{0}{1}", Value, Value is string || Value is char ? "'" : null);
        }
    }
}
