// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using System;
    using NodeType = System.Linq.Expressions.ExpressionType;

    [Serializable]
    public enum UnaryOperator
    {
        ArrayLength = NodeType.ArrayLength,
        Convert = NodeType.Convert,
        ConvertChecked = NodeType.ConvertChecked,
        Negate = NodeType.Negate,
        NegateChecked = NodeType.NegateChecked,
        Not = NodeType.Not,
        Quote = NodeType.Quote,
        TypeAs = NodeType.TypeAs,
        UnaryPlus = NodeType.UnaryPlus,

        PreDecrementAssign = NodeType.PreDecrementAssign,
        PreIncrementAssign = NodeType.PreIncrementAssign,
        PostDecrementAssign = NodeType.PostDecrementAssign,
        PostIncrementAssign = NodeType.PostIncrementAssign,

        Throw = NodeType.Throw,
    }
}
