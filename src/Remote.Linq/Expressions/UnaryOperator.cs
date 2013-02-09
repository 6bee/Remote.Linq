// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;

namespace Remote.Linq.Expressions
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public enum UnaryOperator
    {
        Not,
        IsNull,
        IsNotNull,
    }
}
