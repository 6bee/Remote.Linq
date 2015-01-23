// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using System;

    [Serializable]
    public enum BinaryOperator
    {
        // -- ARITHMETIC --
        Add,
        Subtract,
        Multiply,
        Divide,
        Modulo,
        BitwiseAnd,
        BitwiseOr,
        // -- BOOLEAN --
        ExclusiveOr,
        Coalesce,
        In,
        Contains,
        StartsWith,
        EndsWith,
        And,
        Or,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        Equal,
        NotEqual,
    }
}
