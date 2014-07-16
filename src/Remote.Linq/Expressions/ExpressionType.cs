// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;

namespace Remote.Linq.Expressions
{
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
        MemberInit,
        MethodCall,
        New,
        NewArray,
        Parameter,
        PropertyAccess,
        Sort,
        Unary,
    }
}
