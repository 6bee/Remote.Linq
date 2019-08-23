// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class SortExpression
    {
        public SortExpression()
        {
        }

        public SortExpression(LambdaExpression operand, SortDirection sortDirection)
        {
            Operand = operand;
            SortDirection = sortDirection;
        }

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public LambdaExpression Operand { get; set; }

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = true)]
        public SortDirection SortDirection { get; set; }

        public override string ToString()
            => $"OrderBy ({Operand}) {SortDirection}";
    }
}
