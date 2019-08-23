// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using System;
    using Kind = System.Linq.Expressions.GotoExpressionKind;

    [Serializable]
    public enum GotoExpressionKind
    {
        Break = Kind.Break,
        Continue = Kind.Continue,
        Goto = Kind.Goto,
        Return = Kind.Return,
    }
}
