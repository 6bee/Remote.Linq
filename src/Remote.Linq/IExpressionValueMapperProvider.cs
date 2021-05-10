// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.Dynamic;
    using System;
    using SystemLinq = System.Linq.Expressions;

    public interface IExpressionValueMapperProvider
    {
        IDynamicObjectMapper ValueMapper { get; }

        Func<SystemLinq.Expression, bool>? CanBeEvaluatedLocally { get; }
    }
}