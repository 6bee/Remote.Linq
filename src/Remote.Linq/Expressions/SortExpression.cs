// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    public sealed class SortExpression
    {
        public SortExpression()
        {
        }

        internal SortExpression(LambdaExpression operand, SortDirection sortDirection)
        {
            Operand = operand;
            SortDirection = sortDirection;
        }

        public ExpressionType NodeType { get { return ExpressionType.Sort; } }

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public LambdaExpression Operand { get; set; }

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = true)]
        public SortDirection SortDirection { get; set; }

        public override string ToString()
        {
            return string.Format("OrderBy ({0}) {1}", Operand, SortDirection);
        }
    }
}
