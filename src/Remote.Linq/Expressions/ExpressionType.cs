// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua;
    using System;

    [Serializable]
    public enum ExpressionType
    {
        Binary,
        Collection,
        Conditional,
        Constant,
        Conversion,
        Lambda,
        ListInit,
        Member,
        MemberInit,
        MethodCall,
        New,
        NewArray,
        Parameter,
        Sort,
        TypeIs,
        Unary,
    }
}
