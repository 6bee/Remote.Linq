// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua;
    using System;

    [Serializable]
    public enum UnaryOperator
    {
        Negate,
        Not,
        IsNull,
        IsNotNull,
        Quote,
        TypeAs,
    }
}
