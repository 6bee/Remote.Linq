// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    public sealed class SortExpression
    {
        internal SortExpression(Expression operand, SortDirection sortDirection)
        {
            Operand = operand;
            SortDirection = sortDirection;
        }

        public ExpressionType NodeType { get { return ExpressionType.Sort; } }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public Expression Operand { get; private set; }

        [DataMember(IsRequired = true, EmitDefaultValue = true)]
        public SortDirection SortDirection { get; private set; }

        public override string ToString()
        {
            return string.Format("OrderBy ({0}) {1}", Operand, SortDirection);
        }
    }
}
