// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using System;
    using NodeType = System.Linq.Expressions.ExpressionType;

    [Serializable]
    public enum NewArrayType
    {
        NewArrayBounds = NodeType.NewArrayBounds,
        NewArrayInit = NodeType.NewArrayInit,
    }
}
