// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionVisitors
{
    using System;
    using System.ComponentModel;

    [Obsolete("Class was renamed to " + nameof(SystemExpressionVisitorBase) + ". " + nameof(ExpressionVisitorBase) + " will be removed in a future version.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class ExpressionVisitorBase : SystemExpressionVisitorBase
    {
    }
}